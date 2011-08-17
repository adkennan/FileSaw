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

namespace FileSaw
{
	/// <summary>
	/// Interface implemented by classes that control how text is read from a stream.
	/// </summary>
	public interface IDelimitedParseStrategy
	{
		/// <summary>
		/// Resets the state of the parse strategy.
		/// </summary>
		void Reset();
		
		/// <summary>
		/// Gets a value indicating whether the current character is escaped.
		/// </summary>		
		bool Escaped { get; }
			
		/// <summary>
		/// Indicates whether a character should be included in the extracted data.
		/// </summary>
		/// <param name="valueToInclude">The character to check for inclusion.</param>
		/// <param name="nextValue">The next character in the text stream.</param>
		/// <returns>True if the <paramref name="valueToInclude"/> should be included in the extracted data.</returns>
		bool IncludeChar(char valueToInclude, char? nextValue);
			
	}
}
