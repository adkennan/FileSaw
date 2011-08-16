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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FileSaw
{
	/// <summary>
	/// An <see cref="IRecordReader"/> implementation that reads records of fixed length fields.
	/// </summary>
	public class FixedRecordReader : IRecordReader
	{
		private string _recordDelimiter;
		private int _lastLineLength;
		
		/// <summary>
		/// An <see cref="IRecordReader"/> implementation that reads records of fixed length fields.
		/// </summary>
		/// <param name="recordDelimiter">The string to use as a record delimiter. Usually a newline.</param>
		public FixedRecordReader(string recordDelimiter)
		{
			_recordDelimiter = recordDelimiter;
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
			
			var lineBuilder = _lastLineLength == 0 ? new StringBuilder() : new StringBuilder(_lastLineLength * 2);
			int current = textReader.Read();
			int next = textReader.Peek();
			
			while( current > -1 )
			{
				lineBuilder.Append((char)current);
				if( lineBuilder.EndsWith(_recordDelimiter) ) {
					lineBuilder.Length -= _recordDelimiter.Length;
					break;
				}
		
				current = textReader.Read();
				next = textReader.Peek();
			}
			
			line = lineBuilder.ToString();
			_lastLineLength = line.Length;
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
			var recordSpec = (FixedRecordSpec)context.RecordSpec;
			for( int ix = 0; ix < recordSpec.FieldCount; ix++ ) {
				var fieldSpec = (FixedFieldSpec)recordSpec[ix];
				yield return line.Substring(fieldSpec.Start, fieldSpec.Length);
			}
		}
		
		/// <summary>
		/// Constructs a new RecordSpec instance.
		/// </summary>
		/// <param name="parser">The parser using this instance to extract data.</param>
		/// <param name="name">The name of the record.</param>
		/// <returns>A new RecordSpec.</returns>
		public RecordSpec CreateRecordSpec(Parser parser, string name)
		{
			return new FixedRecordSpec(parser, name);
		}
	}
}
