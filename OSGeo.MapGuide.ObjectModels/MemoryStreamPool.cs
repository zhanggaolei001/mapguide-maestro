﻿#region Disclaimer / License

// Copyright (C) 2017, Jackie Ng
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
using Microsoft.IO;
using System.IO;

namespace OSGeo.MapGuide.ObjectModels
{
    /// <summary>
    /// Provides <see cref="System.IO.MemoryStream"/> instances with pooled buffers to reduce heap fragmentation
    /// and GC pressure
    /// </summary>
    public static class MemoryStreamPool
    {
        static RecyclableMemoryStreamManager msManager;

        static MemoryStreamPool()
        {
            msManager = new RecyclableMemoryStreamManager();
        }

        /// <summary>
        /// Returns a recyclable <see cref="System.IO.MemoryStream"/>
        /// </summary>
        /// <returns></returns>
        public static MemoryStream GetStream() => msManager.GetStream();

        /// <summary>
        /// Returns a recyclable <see cref="System.IO.MemoryStream"/>
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static MemoryStream GetStream(string tag) => msManager.GetStream(tag);

        /// <summary>
        /// Returns a recyclable <see cref="System.IO.MemoryStream"/> initialized with the given buffer
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static MemoryStream GetStream(string tag, byte[] buffer) => GetStream(tag, buffer, 0, buffer.Length);

        /// <summary>
        /// Returns a recyclable <see cref="System.IO.MemoryStream"/> initialized with the given buffer
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static MemoryStream GetStream(string tag, byte[] buffer, int offset, int length) => msManager.GetStream(tag, buffer, offset, length);
    }
}
