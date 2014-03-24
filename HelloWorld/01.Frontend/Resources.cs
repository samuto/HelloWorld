using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.D3DCompiler;
using Device = SlimDX.Direct3D11.Device;
using System.IO;

namespace WindowsFormsApplication7.Frontend
{
    class Resources
    {
        private Dictionary<string, TextureWrapper> textures = new Dictionary<string, TextureWrapper>();
        private Device device;

        private class TextureWrapper
        {
            public Texture2D Texture;
            public ShaderResourceView ShaderResourceView;
        }

        public Resources(Device device)
        {
            this.device = device;
        }

        public void LoadAllTextures()
        {
            string[] files = Directory.GetFiles("01.frontend/Textures", "*");
            foreach (var filename in files)
            {
                string name = Path.GetFileNameWithoutExtension(filename);
                TextureWrapper wrapper = new TextureWrapper();
                wrapper.Texture = Texture2D.FromFile(device, filename);
                wrapper.ShaderResourceView = new ShaderResourceView(device, wrapper.Texture);
                textures.Add(name, wrapper);
            }
        }

        internal ShaderResourceView GetShaderResourceView(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            return textures[name].ShaderResourceView;
        }

        public void Dispose()
        {
            foreach (var wrapper in textures.Values)
            {
                wrapper.Texture.Dispose();
                wrapper.ShaderResourceView.Dispose();
            }
        }

        internal ShaderResourceView GetResource(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            else
                return textures[name].ShaderResourceView;
        }
    }
}
