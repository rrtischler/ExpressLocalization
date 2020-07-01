﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LazZiya.ExpressLocalization.ResxTools
{
    /// <summary>
    /// Resx files writer
    /// </summary>
    public class ResxManager
    {
        private readonly XDocument _xd;

        /// <summary>
        /// Initialize new instance of resource manager
        /// </summary>
        /// <param name="resxType">Type of the resource file</param>
        /// <param name="location">Localization resources folder path</param>
        /// <param name="culture"></param>
        /// <param name="ext"></param>
        public ResxManager(Type resxType, string location = "", string culture = "", string ext = "resx")
            : this(resxType.Name, location, culture, ext)
        {
        }

        /// <summary>
        /// Initialize a new instance of ResxManager
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="location"></param>
        /// <param name="culture"></param>
        /// <param name="ext"></param>
        public ResxManager(string baseName, string location = "", string culture = "", string ext = "resx")
        {
            TargetResourceFile = string.IsNullOrWhiteSpace(culture)
                ? Path.Combine(location, $"{baseName}.{ext}")
                : Path.Combine(location, $"{baseName}.{culture}.{ext}");

            if (File.Exists(TargetResourceFile))
                _xd = XDocument.Load(TargetResourceFile);
        }

        /// <summary>
        /// Retrive the name of the target resource file
        /// </summary>
        public string TargetResourceFile { get; private set; }

        /// <summary>
        /// Add an element to the resource file
        /// </summary>
        /// <param name="element"></param>
        /// <param name="overWriteExistingKeys"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(ResxElement element, bool overWriteExistingKeys = false)
        {
            var elmnt = await FindAsync(element.Key);
            var tsk = new TaskCompletionSource<bool>();

            if (elmnt == null)
            {
                try
                {
                    _xd.Root.Add(element.ToXElement());
                    tsk.SetResult(true);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            if (elmnt != null && overWriteExistingKeys == false)
            {
                tsk.SetResult(false);
            }

            if (elmnt != null && overWriteExistingKeys == true)
            {
                try
                {
                    _xd.Root.Elements("data").FirstOrDefault(x => x == elmnt).ReplaceWith(element.ToXElement());
                    tsk.SetResult(true);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return await tsk.Task;
        }

        /// <summary>
        /// Add array of elements to the resource file
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="overWriteExistingKeys"></param>
        /// <returns></returns>
        public async Task<int> AddRangeAsync(IEnumerable<ResxElement> elements, bool overWriteExistingKeys = false)
        {
            var total = 0;

            foreach (var e in elements.Distinct())
            {
                var success = await AddAsync(e, overWriteExistingKeys);
                
                if (success)
                    total++;
            }

            return total;
        }

        /// <summary>
        /// Async delete a resource key from resource file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteAsync(string key)
        {
            var elmnt = await FindAsync(key);

            if (elmnt == null)
            {
                return null;
            }

            var tsk = new TaskCompletionSource<bool>();

            try
            {
                _xd.Root.Elements("data").FirstOrDefault(x => x == elmnt).Remove();
                tsk.SetResult(true);
            }
            catch (Exception e)
            {
                tsk.SetResult(false);
            }

            return await tsk.Task;
        }

        /// <summary>
        /// Find resource by its key value
        /// </summary>
        /// <param name="key"></param>
        /// <returns>XElement</returns>
        public async Task<XElement> FindAsync(string key)
        {
            var tsk = new TaskCompletionSource<XElement>();

            await Task.Run(() =>
            {
                var elmnt = _xd.Root.Descendants("data").FirstOrDefault(x => x.Attribute("name").Value.Equals(key, StringComparison.OrdinalIgnoreCase));

                if (elmnt == null)
                    tsk.SetResult(null);

                else
                    tsk.SetResult(elmnt);
            });

            return await tsk.Task;
        }

        /// <summary>
        /// Find resource by its key value
        /// </summary>
        /// <param name="key"></param>
        /// <returns>XElement</returns>
        public XElement Find(string key)
        {
            var elmnt = _xd?.Root.Elements("data").FirstOrDefault(x => x.Attribute("name").Value.Equals(key, StringComparison.OrdinalIgnoreCase));

            return elmnt;
        }

        /// <summary>
        /// TryGet method : use inside localizers
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out string value)
        {
            var elmnt = _xd?.Root.Elements("data").FirstOrDefault(x => x.Attribute("name").Value.Equals(key, StringComparison.OrdinalIgnoreCase));

            value = elmnt?.Element("value").Value ?? null;

            return value != null;
        }

        /// <summary>
        /// Add all elements in xml to resx file. CAUTION: THIS COMMAND WILL RECYCLE THE APPPOOL! 
        /// </summary>
        /// <param name="onlyApproved"></param>
        /// <returns></returns>
        public async Task<int> ToResxAsync(bool onlyApproved = false)
        {
            var sourceXmlFile = TargetResourceFile.Replace(".resx", ".xml");

            if (!File.Exists(TargetResourceFile))
            {
                try
                {
                    var dummyFileLoc = typeof(ResxTemplate).Assembly.Location;
                    var dummyFile = $"{dummyFileLoc.Substring(0, dummyFileLoc.LastIndexOf('\\'))}\\ResxTools\\{nameof(ResxTemplate)}.resx";
                    File.Copy(dummyFile, TargetResourceFile);
                }
                catch (Exception e)
                {
                    throw new FileLoadException($"Can't load or create resource file. {e.Message}");
                }
            }

            var xmlRes = XDocument.Load(sourceXmlFile);

            var elements = onlyApproved
                ? xmlRes.Root.Descendants("data")
                             .Where(x => x.Attribute("isActive").Value == "true")
                             .Select(x => new ResxElement
                             {
                                 Key = x.Element("key")?.Value,
                                 Value = x.Element("value")?.Value,
                                 Comment = x.Element("comment")?.Value
                             })
                : xmlRes.Root.Descendants("data")
                             .Select(x => new ResxElement
                             {
                                 Key = x.Element("key")?.Value,
                                 Value = x.Element("value")?.Value,
                                 Comment = x.Element("comment")?.Value
                             });

            var totalAdded = await AddRangeAsync(elements);

            if (totalAdded > 0)
            {
                var success = await SaveAsync();

                if (!success)
                    totalAdded = 0;
            }

            return totalAdded;
        }

        /// <summary>
        /// save the resource file
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            var tsk = new TaskCompletionSource<bool>();

            try
            {
                _xd.Save(TargetResourceFile);
                tsk.SetResult(true);
            }
            catch (Exception e)
            {
                tsk.SetResult(false);
            }

            return await tsk.Task;
        }
    }
}
