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
    class TextureList
    {
        private Dictionary<string, TextureWrapper> textures = new Dictionary<string, TextureWrapper>();
        private Device device;

        private class TextureWrapper
        {
            public Texture2D Texture;
            public ShaderResourceView ShaderResourceView;
        }

        public TextureList(Device device)
        {
            this.device = device;
        }

        public void Load(string name)
        {
            string[] files = Directory.GetFiles("01.frontend/Textures/", name + ".*");
            if(files.Length!=1)
                throw new FileNotFoundException("unknown texture: "+name);
            string filename = files[0];
            TextureWrapper wrapper = new TextureWrapper();
            wrapper.Texture = Texture2D.FromFile(device, filename);
            wrapper.ShaderResourceView = new ShaderResourceView(device, wrapper.Texture);
            textures.Add(name, wrapper);
        }

        internal ShaderResourceView GetShaderResourceView(string name)
        {
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
    }
}
