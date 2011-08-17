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
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace FileSaw
{
	/// <summary>
	/// A dynamic text parser.
	/// </summary>
	public class Parser : DynamicObject
	{
		private readonly List<RecordSpec> _records = new List<RecordSpec>();
		private readonly IRecordReader _recordReader;
		private IRecordFactory _recordFactory = new DefaultRecordFactory();
		
		/// <summary>
		/// A dynamic text parser.
		/// </summary>
		public Parser(IRecordReader recordReader)
		{
			_recordReader = recordReader;
		}
		
		/// <summary>
		/// Sets the record factory used by this parser.
		/// </summary>
		/// <param name="recordFactory"></param>
		public void SetRecordFactory(IRecordFactory recordFactory)
		{
			if( recordFactory == null ) {
				throw new ArgumentNullException("recordFactory");
			}
			_recordFactory = recordFactory;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = null;
			
			var recordSpec = _records.Find(rs => rs.RecordName == binder.Name);
			if(recordSpec != null )
			{
				throw new InvalidOperationException("A record named [" + binder.Name + "] has already been described.");
			}
			
			recordSpec = _recordReader.CreateRecordSpec(binder.Name);
			_records.Add(recordSpec);
			result = recordSpec;
			return true;
		}
		
		private void ParseRecord(ParserContext context)
		{
			int ix = 0;
			foreach (var value in _recordReader.ReadValues(context.LineValue, context)) {
			 	context.Current.Add(context.RecordSpec[ix].ConvertText(value));
				ix++;
			}
		}
		
		private string ReadLine(TextReader reader)
		{
			string line;
			if( _recordReader.TryExtractLine(reader, out line))
			{
				return line;
			}
			return null;
		}
		
		private RecordSpec FindRecordSpec(ParserContext context)
		{
			return _records.FirstOrDefault(rs => rs.IsMatch(context));
		}
		
		internal bool HasRecordNamed(string name)
		{
			return _records.Any(rs => rs.RecordName == name);
		}
		
		/// <summary>
		/// Parse records from the supplied text reader.
		/// </summary>
		/// <param name="reader">The text reader to read records from.</param>
		/// <returns>A collection records</returns>
		public IEnumerable<Record> Parse(TextReader reader)
		{
			var context = new ParserContext 
			{ 
				Parser = this,
				FirstRecord = true
			};
			
			var currentLine = ReadLine(reader);
			var nextLine = ReadLine(reader);
			
			while( currentLine != null )
			{
				context.LineValue = currentLine;
				context.LastRecord = nextLine == null;
				context.RecordSpec = FindRecordSpec(context);
				context.Current = _recordFactory.CreateRecord(context);
				
				ParseRecord(context);
				
				context.Current.Validate(context);
				
				yield return context.Current;
				
				context.FirstRecord = false;
				
				currentLine = nextLine;
				nextLine = ReadLine(reader);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static dynamic CsvWithQuotedStrings()
		{
			return CsvWithQuotedStrings(Environment.NewLine);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newLine"></param>
		/// <returns></returns>
		public static dynamic CsvWithQuotedStrings(string newLine)
		{
			return DelimitedWithQuotedStrings(",", newLine);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static dynamic Csv()
		{
			return Csv(Environment.NewLine);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newLine"></param>
		/// <returns></returns>
		public static dynamic Csv(string newLine)
		{
			return Csv(newLine, new BasicDelimitedParseStrategy());
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newLine"></param>
		/// <param name="parseStrategy"></param>
		/// <returns></returns>
		public static dynamic Csv(string newLine, IDelimitedParseStrategy parseStrategy)
		{
			return Delimited(",", newLine, parseStrategy);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public static dynamic Delimited(string delimiter)
		{
			return Delimited(delimiter, Environment.NewLine);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delimiter"></param>
		/// <param name="newLine"></param>
		/// <returns></returns>
		public static dynamic Delimited(string delimiter, string newLine)
		{
			return Delimited(delimiter, newLine, new BasicDelimitedParseStrategy());
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public static dynamic DelimitedWithQuotedStrings(string delimiter)
		{
			return DelimitedWithQuotedStrings(delimiter, Environment.NewLine);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delimiter"></param>
		/// <param name="newLine"></param>
		/// <returns></returns>
		public static dynamic DelimitedWithQuotedStrings(string delimiter, string newLine)
		{
			return Delimited(delimiter, newLine, new QuotedStringParseStrategy());
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="delimiter"></param>
		/// <param name="newLine"></param>
		/// <param name="parseStrategy"></param>
		/// <returns></returns>
		public static dynamic Delimited(string delimiter, string newLine, IDelimitedParseStrategy parseStrategy) {
			
			return new Parser(new DelimitedRecordReader(delimiter, newLine, parseStrategy));
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static dynamic Fixed()
		{
			return Fixed(Environment.NewLine);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newLine"></param>
		/// <returns></returns>
		public static dynamic Fixed(string newLine)
		{
			return new Parser(new FixedRecordReader(newLine));
		}
	}
}
