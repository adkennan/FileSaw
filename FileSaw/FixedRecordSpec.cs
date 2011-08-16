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
	/// Describes a fixed length record.
	/// </summary>
	public class FixedRecordSpec : RecordSpec
	{
		/// <summary>
		/// Describes a fixed length record.
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <param name="name">The name of the record.</param>
		public FixedRecordSpec(Parser parser, string name)
			: base(parser, name)
		{
		}
		
		/// <summary>
		/// Creates a field specification instance.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <returns></returns>
		protected override FieldSpec CreateFieldSpec(string name)
		{
			return new FixedFieldSpec(name);
		}
		
		private new FixedFieldSpec CurrentField { get { return (FixedFieldSpec)base.CurrentField; } }
		
		/// <summary>
		/// Sets the start position and length of the current field in the record.
		/// </summary>
		/// <param name="start">The position of the first character of the field.</param>
		/// <param name="length">The length of the field in characters.</param>
		/// <returns></returns>
		public dynamic At(int start, int length)
		{
			CurrentField.Start = start;
			CurrentField.Length = length;
			
			return this;
		}
	}
}
