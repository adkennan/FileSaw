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
	/// Description of RecordSpec.
	/// </summary>
	public class RecordSpec : DynamicObject
	{
		private readonly string _name;
		private readonly List<FieldSpec> _fields = new List<FieldSpec>();
		private Predicate<ParserContext> _where;
		private Predicate<ParserContext> _requiring;
		private FieldSpec _currentField;
		
		internal RecordSpec(string name)
		{
			_name = name;
		}
		
		internal string RecordName { get { return _name; } }
		
		internal int GetIndex(string name)
		{
			return _fields.FindIndex(fs => fs.Name == name);
		}
		
		internal FieldSpec this[int index]
		{
			get { return _fields[index]; } 
		}
		
		internal int FieldCount { get { return _fields.Count; } }
		
		internal bool IsMatch(ParserContext context)
		{
			if( _where == null )
			{
				return true;
			}
			return _where(context);
		}
		
		internal bool IsValid(ParserContext context)
		{
			if( _requiring == null ) 
			{
				return true;
			}
			return _requiring(context);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			var fieldSpec = _fields.Find(fs => fs.Name == binder.Name);
			
			if( fieldSpec != null ) {
				throw new InvalidOperationException("A field named [" + binder.Name + "] has already been described.");
			}
			
			fieldSpec = CreateFieldSpec(binder.Name);
			_fields.Add(fieldSpec);
			result = this;
			_currentField = fieldSpec;
			return true;
		}
		
		/// <summary>
		/// Creates a new FieldSpec. 
		/// Override this in derived classes to provide specialized FieldSpecs.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected virtual FieldSpec CreateFieldSpec(string name)
		{
			return new FieldSpec(name);
		}
		
		/// <summary>
		/// Sets a predicate used to identify the type of a record.
		/// </summary>
		/// <param name="predicate">The function called to identify a record. 
		/// Returns true if the record is a match.</param>
		/// <returns></returns>
		public dynamic Where(Predicate<ParserContext> predicate)
		{
			if( _where != null ) {
				throw new InvalidOperationException("Where already specified.");
			}
			_where = predicate;
			_currentField = null;
			return this;
		}
		
		/// <summary>
		/// Sets a predicate used to validate a record after it has been parsed.
		/// </summary>
		/// <param name="predicate">The function called to identify a record.
		/// Returns true if the record is valid.
		/// </param>
		/// <returns></returns>
		public dynamic Requiring(Predicate<ParserContext> predicate)
		{
			if( _requiring != null ) {
				throw new InvalidOperationException("Requiring already specified.");
			}
			_requiring = predicate;
			_currentField = null;
			return this;
		}
		
		/// <summary>
		/// Sets the type of a field in the record. 
		/// </summary>
		/// <returns></returns>
		public dynamic As<T>() where T : IConvertible
		{
			if( _currentField == null ) {
				throw new InvalidOperationException("No current field.");
			}
			_currentField.SetConverter(val => Convert.ChangeType(val, typeof(T)));
			return this;
		}
		
		/// <summary>
		/// Sets the type of a field in the record. 
		/// </summary>
		/// <param name="convert">A conversion function used to convert the string read from 
		/// the text stream into the correct type.</param>
		/// <returns></returns>
		public dynamic As<T>(Func<string, T> convert)
		{
			if( _currentField == null ) {
				throw new InvalidOperationException("No current field.");
			}
			_currentField.SetConverter(val => (object)convert(val));
			return this;
		}
		
		/// <summary>
		/// Gets the field that is currently being defined.
		/// </summary>
		protected FieldSpec CurrentField { get { return _currentField; } } 
	}
}
