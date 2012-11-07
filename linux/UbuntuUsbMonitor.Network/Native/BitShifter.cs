using System;
using System.Runtime.InteropServices;

namespace DevTest
{
	/// <summary>
	/// Bit shifter.
	/// </summary>
	/// <remarks>
	/// Some code implementation from /usr/include/asm-generic/ioctl.h
	/// </remarks>
	internal static class BitShifter
	{
		private const int MON_IOC_MAGIC	= 0x92;
		private const int _IOC_NONE		=	0;
		private const int _IOC_WRITE		=	1;
		private const int _IOC_READ		=	2;

		private const int _IOC_NRBITS	=	8;
		private const int _IOC_SIZEBITS	=	14;
		private const int _IOC_DIRBITS	=	2;
		private const int _IOC_TYPEBITS	=	8;

		private const int _IOC_NRSHIFT		=	0;
		private const int _IOC_TYPESHIFT	=	_IOC_NRSHIFT + _IOC_NRBITS;
		private const int _IOC_SIZESHIFT	=	_IOC_TYPESHIFT + _IOC_TYPEBITS;
		private const int _IOC_DIRSHIFT		=	_IOC_SIZESHIFT + _IOC_SIZEBITS;

		private static int _IOC (int dir, int type, int nr, int size)
		{
			return (dir << _IOC_DIRSHIFT) | (type << _IOC_TYPESHIFT) | (nr << _IOC_NRSHIFT) | (size << _IOC_SIZESHIFT);
		}

		internal static int _IO(int type = MON_IOC_MAGIC, int nr = 0 )
		{
			return _IOC(_IOC_NONE,type,nr,0);
		}

		internal static int _IOR (int type = MON_IOC_MAGIC, int nr = 0, int SizeOfType = 0)
		{
			return  _IOC(_IOC_READ, type, nr, SizeOfType);
		}

		internal static int _IOW (int type = MON_IOC_MAGIC, int nr = 0, int SizeOfType = 0)
		{
			return _IOC (_IOC_WRITE, type, nr, SizeOfType);
		}
	}
}