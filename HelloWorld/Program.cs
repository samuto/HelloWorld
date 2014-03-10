using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.D3DCompiler;
using Device = SlimDX.Direct3D11.Device;

namespace WindowsFormsApplication7
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            TheGame.Instance.Run();
        }
    }
}