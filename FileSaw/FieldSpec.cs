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
using System.Dynamic;

namespace FileSaw
{
	/// <summary>
	/// Describes a field in a record.
	/// </summary>
	public class FieldSpec
	{
		private readonly string _name;
		private Func<string, object> _converter;
		
		/// <summary>
		/// Describes a field in a record.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		public FieldSpec(string name)
		{
			_name = name;
		}
		
		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		public string Name { get { return _name; } }
		
		/// <summary>
		/// Converts the raw string value extracted from the text stream 
		/// into the correct type.
		/// </summary>
		/// <param name="value">The string value to convert.</param>
		/// <returns>The value converted to the correct type.</returns>
		public object ConvertText(string value)
		{
			if( _converter == null ) {
				return value;
			}
			return _converter(value);
		}
		
		/// <summary>
		/// Sets the function used to convert values from the text stream
		/// into the correct type.
		/// </summary>
		/// <param name="converter">The conversion function to use.</param>
		internal void SetConverter(Func<string, object> converter)
		{
			_converter = converter;
		}
		                           
	}
}
