namespace Bowerbird.Core.Config
{
    public interface ISystemStateManager
    {
        void SetupSystem();
        void SwitchServicesOff();
        void SwitchServicesOn();

        void SwitchServices(bool? enableEmailService = null,
                            bool? enableBackChannelService = null,
                            bool? enableImageService = null,
                            bool? enableYouTubeVideoService = null,
                            bool? enableVimeoVideoService = null,
                            bool? enableDocumentService = null,
                            bool? enableAudioService = null);
    }
}
