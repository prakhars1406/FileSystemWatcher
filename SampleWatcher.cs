        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void Runn()
        {
             MessageBox.Show("File Monitor");
            
            Task.Run(async () =>
                {
                    using (FileSystemWatcher watcher = new FileSystemWatcher())
                    {
                        watcher.Path = System.IO.Path.GetTempPath() + "..\\path";//this path will be wathced
                        // Watch for changes in LastAccess and LastWrite times, and
                        // the renaming of files or directories.
                        watcher.NotifyFilter = NotifyFilters.LastWrite;
                        // Only watch text files.
                        watcher.Filter = "TempHashCache.xml";
                        // Add event handlers.
                        watcher.Changed += OnChanged;
                        // Begin watching.
                        watcher.EnableRaisingEvents = true;
                        while (true)
                        {
                            await Task.Delay(3000);
                        }
                    }
                });
            
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
            //    MessageBox.Show(e.FullPath);
                XmlDocument doc = new XmlDocument();
                doc.Load(e.FullPath);
                string hash = doc.SelectSingleNode("string").InnerText;
             //   MessageBox.Show(hash.ToLower());
                File.Delete(e.FullPath);
                var foldersFile = Path.Combine(settingsPath, "TempHashCache.xml");
                if (!File.Exists(foldersFile))
                {
                    var xmlWriter = XmlWriter.Create(foldersFile);
                    xmlWriter.Close();
                }
                var hashCacheHandler = iocContainer.Resolve<HashCacheHandler>();
                hashCacheHandler.HashCache[hash.ToLower()].Status = HashValidationStatus.Success;
                // Remove all attributes and child nodes from the book element.
                //   doc.SelectSingleNode("string").InnerText=null;
            }
            catch(Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }
        }
