using System;

namespace SmartRoadSense.Shared.Data {

    public class FileGeneratedEventArgs : EventArgs {

        public FileGeneratedEventArgs(string filepath) {
            Filepath = filepath;
        }

        public string Filepath { get; private set; }

    }

}

