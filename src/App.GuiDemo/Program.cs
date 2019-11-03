// This file is part of Abanu, an Operating System written in C#. Web: https://www.abanu.org
// Licensed under the GNU 2.0 license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Abanu.Kernel.Core;
using Abanu.Runtime;
using Mosa.Runtime.x86;

namespace Abanu.Kernel
{

    public static class Program
    {

        private static ISurface sur;
        private static IGraphicsAdapter gfx;

        public static unsafe void Main()
        {
            ApplicationRuntime.Init();
            MessageManager.OnDispatchError = OnDispatchError;
            MessageManager.OnMessageReceived = MessageReceived;

            Console.WriteLine("Gui Demo starting");

            var targetProcessID = SysCalls.GetProcessIDForCommand(SysCallTarget.Tmp_DisplayServer_CreateWindow);
            var windowData = (CreateWindowResult*)SysCalls.RequestMessageBuffer((uint)sizeof(CreateWindowResult), targetProcessID).Start;

            SysCalls.Tmp_DisplayServer_CreateWindow(ApplicationRuntime.CurrentProcessID, windowData, 200, 100);
            sur = new MemorySurface(windowData->Addr, windowData->Width, windowData->Height, windowData->Pitch, windowData->Depth);
            gfx = new GraphicsAdapter(sur);

            gfx.SetSource(0x0000FF00);
            gfx.Rectangle(0, 0, sur.Width, sur.Height);
            gfx.Fill();

            SysCalls.Tmp_DisplayServer_FlushWindow();

            Console.WriteLine("Gui Demo ready");

            var direction = 1;
            var pos = 128;
            gfx.MoveTo(10, 10);
            gfx.LineTo(50, 70);

            while (true)
            {
                uint color = 0x0000FF00;
                uint mask = (uint)pos;
                uint newColor = color | mask;

                gfx.SetSource(newColor);
                gfx.Rectangle(0, 0, sur.Width, sur.Height);
                gfx.Fill();

                gfx.SetSource(0x00FF0000);
                gfx.Stroke();

                pos += direction;
                if (pos >= 255)
                {
                    direction = -1;
                }
                else
                {
                    if (pos <= 0)
                        direction = 1;
                }

                SysCalls.Tmp_DisplayServer_FlushWindow();

                //SysCalls.ThreadSleep(0);
            }
        }

        public static unsafe void OnDispatchError(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        public static unsafe void MessageReceived(SystemMessage* msg)
        {
            MessageManager.Send(new SystemMessage(SysCallTarget.ServiceReturn));
        }

    }
}