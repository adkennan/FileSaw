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

namespace FileSaw
{
	/// <summary>
	/// Interface implemented by classes that describe how records a read from a text stream.
	/// </summary>
	public interface IRecordReader
	{
		/// <summary>
		/// Attempts to extract the next text record from the supplied TextReader.
		/// </summary>
		/// <param name="textReader">The TextReader to read data from.</param>
		/// <param name="line">The record text read from the TextReader.</param>
		/// <returns>True if a record was successfully read or false if no characters could be read.</returns>
		bool TryExtractLine(TextReader textReader, out string line);
		
		/// <summary>
		/// Extracts a collection of field values from the supplied text record.
		/// </summary>
		/// <param name="line">The text record to extract field values from.</param>
		/// <param name="context">The current parser context.</param>
		/// <returns>A collection of string values corresponding to the fields in the file definition.</returns>
		IEnumerable<string> ReadValues(string line, ParserContext context);
	
		/// <summary>
		/// Constructs a new RecordSpec instance.
		/// </summary>
		/// <param name="name">The name of the record.</param>
		/// <returns>A new RecordSpec.</returns>
		RecordSpec CreateRecordSpec(string name);
	}
}
