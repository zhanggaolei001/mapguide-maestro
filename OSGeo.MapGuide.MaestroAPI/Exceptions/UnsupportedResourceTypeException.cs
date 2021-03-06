﻿#region Disclaimer / License

// Copyright (C) 2010, Jackie Ng
// https://github.com/jumpinjackie/mapguide-maestro
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//

#endregion Disclaimer / License

using System;

namespace OSGeo.MapGuide.MaestroAPI.Exceptions
{
    /// <summary>
    /// Used to indicate a resource type is not supported for a connection
    /// </summary>
    [global::System.Serializable]
    public class UnsupportedResourceTypeException : MaestroException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedResourceTypeException"/> class.
        /// </summary>
        /// <param name="rt">The resource type.</param>
        public UnsupportedResourceTypeException(string rt)
        {
            this.ResourceType = rt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedResourceTypeException"/> class.
        /// </summary>
        /// <param name="rt">The resource type.</param>
        /// <param name="message">The message.</param>
        public UnsupportedResourceTypeException(string rt, string message)
            : base(message)
        {
            this.ResourceType = rt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedResourceTypeException"/> class.
        /// </summary>
        /// <param name="rt">The resource type.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public UnsupportedResourceTypeException(string rt, string message, Exception inner)
            : base(message, inner)
        {
            this.ResourceType = rt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedResourceTypeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected UnsupportedResourceTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Gets or sets the type of the resource.
        /// </summary>
        /// <value>The type of the resource.</value>
        public string ResourceType { get; }
    }
}