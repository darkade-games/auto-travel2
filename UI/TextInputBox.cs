using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AutoTravel2.UI
{
    public class TextInputBox : StardewValley.Menus.TextBox
    {

        public event EventHandler<string> onTextReceived;

        public TextInputBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor) : base(textBoxTexture, caretTexture, font, textColor)
        {

        }

        public override void RecieveTextInput(char inputChar)
        {
            base.RecieveTextInput(inputChar);
            if (onTextReceived != null)
            {
                onTextReceived.Invoke(this, inputChar.ToString());
            }
        }

        public override void RecieveTextInput(string text)
        {
            base.RecieveTextInput(text);
            if (onTextReceived != null)
            {
                onTextReceived.Invoke(this, text);
            }
        }

        public virtual void backSpacePressed()
        {
            Text = Text.Remove(Text.Length - 1);
        }
    }
}
