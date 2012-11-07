UbuntuUsbMonitor
================

Small learning Mono/C# project to use client/server technology (based on ZeroMQ) to monitor USB events on Linux and send them to Windows WPF client

To start working on Linux side you need to follow those steps (original article here: http://www.kernel.org/doc/Documentation/usb/usbmon.txt)

1. Prepare

Mount debugfs (it has to be enabled in your kernel configuration), and 	load the usbmon module (if built as module). The second step is skipped 	if usbmon is built into the kernel.

# mount -t debugfs none_debugs /sys/kernel/debug
# modprobe usbmon
#

Verify that bus sockets are present.

# ls /sys/kernel/debug/usb/usbmon
0s  0u  1s  1t  1u  2s  2t  2u  3s  3t  3u  4s  4t  4u
#


On my Ubuntu [Ubuntu 12.04.1 LTS] there new devices come up /dev/usbmonX  X means bus number. In this project I user usb device connected to bus 1.

The architecture is simple:
	Make P/Invoke call to Ioctl(...) to receive numbers of USB events waiting in queue and collect them
	Raw events converted to UsbMonPacket and serialized to XML
	XML object serialized to byte [] array and send via ZeroMQ socket to Win32 port 5555
	Win32 (server) listening (via Replay socket) on port 5555 and basically to reverse to above steps (de-serialize to XML and then to actuall UsbMonPacket)
	When UsbMonPacket finally constructed then new event rised and collected by .GUI project
	Then it should be finally presented on screen and user notify about it

P.S. As for now [07-November-2012] there's a lot of TODOs and event broken/bugged classes. I'm working on them on my spare time so it will be fixed slowly but general idea should work.  Enjoy !

