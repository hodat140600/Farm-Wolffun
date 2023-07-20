using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    public class WinMenu : UIPanel
    {
        private static WinMenu instance;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }
        
        public void OnClickLoad()
        {
            TheGame.Get().Pause();
            TheGame.Load();
        }

        public void OnClickNew()
        {
            TheGame.Get().Pause();
            TheGame.NewGame();
        }

        public void OnClickBack()
        {
            Hide();
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            TheGame.Get().Pause();
        }

        public override void Hide(bool instant = false)
        {
            base.Hide(instant);
            TheGame.Get().Unpause();
        }

        public static WinMenu Get()
        {
            return instance;
        }
    }
}