﻿using ImGuiNET;


namespace Wordsmith.Gui
{
    internal class ErrorWindow : MessageBox
    {
        private const string message = "Wordsmith has encountered an error.\nCopy error dump to clipboard and open bug report page?\n\nWARNING: I WILL be able to see anything and everything typed as part of the log.";
        protected Dictionary<string, object> _dump = new Dictionary<string, object>();
        public ErrorWindow( Dictionary<string, object> dump ) : base( $"Wordsmith Error", message, Callback, buttonStyle: ButtonStyle.YesNo) { this._dump = dump; }

        public static void Callback(MessageBox mb)
        {
            if ( mb is ErrorWindow ew )
            {
                if ( ew.Result == DialogResult.Yes )
                {
                    ImGui.SetClipboardText( System.Text.Json.JsonSerializer.Serialize( ew._dump, new System.Text.Json.JsonSerializerOptions() { IncludeFields = true } ) );
                    System.Diagnostics.Process.Start( new System.Diagnostics.ProcessStartInfo( "https://github.com/LadyDefile/Wordsmith-DalamudPlugin/issues" ) { UseShellExecute = true } );
                }
            }
            WordsmithUI.RemoveWindow( mb );
        }
    }
}
