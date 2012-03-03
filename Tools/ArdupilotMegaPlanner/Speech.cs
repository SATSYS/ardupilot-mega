﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;

namespace ArdupilotMega
{
    public class Speech
    {
        SpeechSynthesizer _speechwindows;
        System.Diagnostics.Process _speechlinux;

        System.Speech.Synthesis.SynthesizerState _state = SynthesizerState.Ready;

        bool MONO = false;

        public SynthesizerState State { 
            get {
                if (MONO)
                {
                    return _state;
                }
                else
                {
                    return _speechwindows.State;
                }
            }
        }

        public Speech()
        {
            var t = Type.GetType("Mono.Runtime");
            MONO = (t != null);

            if (MONO)
            {
                _state = SynthesizerState.Ready;
            }
            else
            {
                _speechwindows = new SpeechSynthesizer();
            }
        }

        public void SpeakAsync(string text)
        {
            if (MONO)
            {
                if (_speechlinux == null || _speechlinux.HasExited)
                {
                    _state = SynthesizerState.Speaking;
                    _speechlinux = new System.Diagnostics.Process();
                    _speechlinux.StartInfo.FileName = "echo " + text + " | festival --tts";
                    _speechlinux.Start();
                    _speechlinux.Exited += new EventHandler(_speechlinux_Exited);
                }
            }
            else
            {
                _speechwindows.SpeakAsync(text);
            }
        }

        void _speechlinux_Exited(object sender, EventArgs e)
        {
            _state = SynthesizerState.Ready;
        }

        public void SpeakAsyncCancelAll()
        {
            if (MONO)
            {
                _speechlinux.Close();
                _state = SynthesizerState.Ready;
            }
            else
            {
                _speechwindows.SpeakAsyncCancelAll();
            }
        }
    }
}