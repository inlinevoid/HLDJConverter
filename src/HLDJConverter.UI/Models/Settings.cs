using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLDJConverter.UI.Models
{
    public sealed class Settings: Notifiable
    {
        private string mOutputFolder = "";
        public string OutputFolder
        {
            get { return mOutputFolder; }
            set { SetField(ref mOutputFolder, value); }
        }

        private double mOutputVolume = 1.0;
        public double OutputVolume
        {
            get { return mOutputVolume;}
            set { SetField(ref mOutputVolume, value); }
        }
        
        private int mOutputBitrate = 22050;
        public int OutputBitrate
        {
            get { return mOutputBitrate; }
            set { SetField(ref mOutputBitrate, value); }
        }

        private bool mUploadCrashGist = true;
        public bool UploadCrashGist
        {
            get { return mUploadCrashGist; }
            set {SetField(ref mUploadCrashGist, value); }
        }

        private bool mKeepWindowTopmost = false;
        public bool KeepWindowTopmost
        {
            get { return mKeepWindowTopmost; }
            set { SetField(ref mKeepWindowTopmost, value); }
        }
    }
}
