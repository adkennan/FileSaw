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

namespace FileSaw
{
	/// <summary>
	/// Description of Record.
	/// </summary>
	public class Record : DynamicObject
	{
		private ParserContext _context;
		private readonly List<object> _values = new List<object>();
		
		internal Record()
		{ }
		
		internal void Initialize(ParserContext context)
		{
			_context = context;
			_values.Clear();
		}
		
		/// <summary>
		/// Gets a value indicating whether this is the first record in the set.
		/// </summary>
		public bool FirstRecord { get { return _context.FirstRecord; } }
		
		/// <summary>
		/// Gets a value indicating whether this is the last record in the set.
		/// </summary>
		public bool LastRecord { get { return _context.LastRecord; } }
		
		internal void Add(object value)
		{
			_values.Add(value);
		}
		
		internal void Validate(ParserContext context)
		{
			if( ! _context.RecordSpec.IsValid(context) )
			{
				throw new InvalidOperationException("Invalid!");
			}
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
			
			if( binder.Name.StartsWith("Is", StringComparison.Ordinal) )
			{
				if( binder.Name == "Is" + _context.RecordSpec.RecordName ) {
					result = true;
					return true;
				}
				
				if( _context.Parser.HasRecordNamed(binder.Name.Substring(2))) {
					result = false;
					return true;
				}
			}
			
			var idx = _context.RecordSpec.GetIndex(binder.Name);
			if( idx < 0 ) {
				return false;
			}
			if( idx >= _values.Count ) {
				throw new InvalidOperationException("Value of [" + binder.Name + "] has not been initialized.");
			}
			
			result = _values[idx];
			return true;
		}
	}
}
