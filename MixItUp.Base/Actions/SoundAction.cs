﻿using MixItUp.Base.ViewModel.User;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MixItUp.Base.Actions
{
    [DataContract]
    public class SoundAction : ActionBase
    {
        public const string DefaultAudioDevice = "Default Output";

        private static SemaphoreSlim asyncSemaphore = new SemaphoreSlim(1);

        protected override SemaphoreSlim AsyncSemaphore { get { return SoundAction.asyncSemaphore; } }

        [DataMember]
        public string FilePath { get; set; }

        [DataMember]
        public int VolumeScale { get; set; }

        [DataMember]
        public string OutputDevice { get; set; }

        public SoundAction() : base(ActionTypeEnum.Sound) { this.OutputDevice = null; }

        public SoundAction(string filePath, int volumeScale, string outputDevice = null)
            : this()
        {
            this.FilePath = filePath;
            this.VolumeScale = volumeScale;
            this.OutputDevice = outputDevice;
        }

        protected override async Task PerformInternal(UserViewModel user, IEnumerable<string> arguments)
        {
            if (File.Exists(this.FilePath))
            {
                int audioDevice = -1;
                if (!string.IsNullOrEmpty(this.OutputDevice))
                {
                    audioDevice = await ChannelSession.Services.AudioService.GetOutputDevice(this.OutputDevice);
                }
                await ChannelSession.Services.AudioService.Play(this.FilePath, this.VolumeScale, audioDevice);
            }
        }
    }
}
