﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NDSUtils
{
    [Serializable]
    public class NDSDirectory
    {
        public NDSFilesystem Filesystem { get; private set; }

        /// <summary>
        /// The parent of this directory, or null if this directory is the root one.
        /// </summary>
        public NDSDirectory Parent { get; set; }
        public string FullPath => (Parent?.FullPath ?? "") + "/" + Name;
        public List<NDSDirectory> ChildrenDirectories { get; private set; } = new List<NDSDirectory>();
        public List<NDSFile> ChildrenFiles { get; private set; } = new List<NDSFile>();

        /// <summary>
        /// The entry ID of this directory in the FAT.
        /// </summary>
        public ushort EntryID { get; private set; }

        /// <summary>
        /// The name of this directory.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Locate and create a directory from the root of a ROM file.
        /// </summary>
        /// <param name="rom">The ROM to use.</param>
        public NDSDirectory(NDSFilesystem fs, ushort entryID, string name)
        {
            Filesystem = fs;
            EntryID = entryID;
            Name = name;
        }

        public NDSDirectory GetDirectory(string relativePath)
        {
            Console.WriteLine(relativePath);
            var nextFolder = PathHelpers.GetFirstComponent(relativePath);
            return (from dir in ChildrenDirectories
                    where dir.Name == nextFolder
                    select dir).FirstOrDefault()?.GetDirectory(relativePath.Replace(nextFolder + "/", "")) ?? this;
        }

        public NDSFile GetFile(string relativePath)
        {
            Console.WriteLine(relativePath);
            return (from file in GetDirectory(PathHelpers.GetParent(relativePath)).ChildrenFiles
                    where file.Name == Path.GetFileName(relativePath)
                    select file).FirstOrDefault();
        }
    }
}
