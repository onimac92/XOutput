﻿using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input.Mapper;

namespace XOutput
{
    /// <summary>
    /// Contains the settings that are persisted.
    /// </summary>
    public sealed class Settings
    {
        /// <summary>
        /// Reads a new setting from a file.
        /// </summary>
        /// <param name="filePath">Filepath of the settings file</param>
        /// <returns></returns>
        public static Settings Load(string filePath)
        {
            var settings = new Settings();
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                var devices = text.Split('#');
                foreach(var device in devices)
                {
                    if (string.IsNullOrEmpty(device))
                        continue;
                    var newLineIndex = device.IndexOf(Environment.NewLine);
                    var guid = new Guid(device.Substring(0, newLineIndex));
                    var mapperText = device.Substring(newLineIndex + 1);
                    settings.mappers[guid] = DirectToXInputMapper.Parse(mapperText);
                }
            }
            return settings;
        }
        
        private Dictionary<Guid, DirectToXInputMapper> mappers;

        private Settings()
        {
            mappers = new Dictionary<Guid, DirectToXInputMapper>();
        }

        /// <summary>
        /// Writes the settings into a file.
        /// </summary>
        /// <param name="filePath">Filepath of the settings file</param>
        public void Save(string filePath)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var mapper in mappers)
            {
                sb.Append("#");
                sb.AppendLine(mapper.Key.ToString());
                sb.AppendLine(mapper.Value.ToString());
            }
            File.WriteAllText(filePath, sb.ToString());
        }

        /// <summary>
        /// Gets the mapper with the given deviceID. If the mapper is not saved in this settings, a new mapper will be returned.
        /// </summary>
        /// <param name="id">DeviceID</param>
        /// <returns></returns>
        public DirectToXInputMapper GetMapper(Guid id)
        {
            if(!mappers.ContainsKey(id))
            {
                mappers[id] = new DirectToXInputMapper();
            }
            return mappers[id];
        }
    }
}