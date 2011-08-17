/*
 * FileSaw - A dynamic text file parser.
 * 
 * Copyright 2011 Andrew Kennan. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY ANDREW KENNAN ''AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL ANDREW KENNAN OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Andrew Kennan.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace FileSaw
{
	/// <summary>
	/// An <see cref="IRecordReader"/> implementation that reads delimited formats such as CSV.
	/// </summary>
	public class DelimitedRecordReader : IRecordReader
	{
		private readonly string _fieldDelimiter;
		private readonly string _recordDelimiter;
		private readonly IDelimitedParseStrategy _parseStrategy;
		private readonly StringBuilder _lineBuilder = new StringBuilder();
		
		/// <summary>
		/// An <see cref="IRecordReader"/> implementation that reads delimited formats such as CSV.
		/// </summary>
		/// <param name="fieldDelimiter">The string to use as a field delimiter.</param>		
		/// <param name="recordDelimiter">The string to use as a record delimiter. Usually a newline.</param>
		/// <param name="parseStrategy">The strategy to use when reading text.</param>
		public DelimitedRecordReader(string fieldDelimiter, string recordDelimiter, IDelimitedParseStrategy parseStrategy)
		{
			_fieldDelimiter = fieldDelimiter;
			_recordDelimiter = recordDelimiter;
			_parseStrategy = parseStrategy;
		}
		
		/// <summary>
		/// Attempts to extract the next text record from the supplied TextReader.
		/// </summary>
		/// <param name="textReader">The TextReader to read data from.</param>
		/// <param name="line">The record text read from the TextReader.</param>
		/// <returns>True if a record was successfully read or false if no characters could be read.</returns>
		public bool TryExtractLine(TextReader textReader, out string line)
		{
			if( textReader.Peek() == -1 ) {
				line = null;
				return false;
			}
		
			_parseStrategy.Reset();
			
			_lineBuilder.Clear();
			int current = textReader.Read();
			int next = textReader.Peek();
			
			while( current > -1 )
			{
				char currentChar = (char)current;
				char? nextChar = next >= 0 ? (char)next : default(char?);
				
				if( _parseStrategy.IncludeChar(currentChar, nextChar) )
				{
					_lineBuilder.Append(currentChar);
					if( ! _parseStrategy.Escaped && _lineBuilder.EndsWith(_recordDelimiter) ) {
						_lineBuilder.Length -= _recordDelimiter.Length;
						break;
					}
				}
				
				current = textReader.Read();
				next = textReader.Peek();
			}
			
			line = _lineBuilder.ToString();
			return true;
		}
		
		
		/// <summary>
		/// Extracts a collection of field values from the supplied text record.
		/// </summary>
		/// <param name="line">The text record to extract field values from.</param>
		/// <param name="context">The current parser context.</param>
		/// <returns>A collection of string values corresponding to the fields in the file definition.</returns>
		public IEnumerable<string> ReadValues(string line, ParserContext context)
		{
			_parseStrategy.Reset();
			
			var value = new StringBuilder(line);
			for( int ix = 0; ix < line.Length; ix++ ) {
				char current = line[ix];
				char? next = ix == line.Length - 1 ? default(char?) : line[ix + 1];
				
				if( _parseStrategy.IncludeChar(current, next) ) {
					value.Append(current);
					
					if( ! _parseStrategy.Escaped && value.EndsWith(_fieldDelimiter) ) {
						value.Length -= _fieldDelimiter.Length;
						yield return value.ToString();
						value.Length = 0;
					}
				}
			}
			yield return value.ToString();
		}
		
		/// <summary>
		/// Constructs a new RecordSpec instance.
		/// </summary>
		/// <param name="name">The name of the record.</param>
		/// <returns>A new RecordSpec.</returns>
		public RecordSpec CreateRecordSpec(string name)
		{
			return new RecordSpec(name);
		}
	}
}
