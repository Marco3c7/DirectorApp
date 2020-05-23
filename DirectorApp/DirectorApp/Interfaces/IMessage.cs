using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorApp.Interfaces
{
    public interface IMessage
    {
        void ShowToast(string text);
        void ShowSnackBar(string text);
    }
}
