﻿/*
 * Copyright 2015 (C) Karlsruhe Institute of Technology (KIT)
 * Marc Rittinghaus
 *
 *             _____ _                 __
 *            / ___/(_)___ ___  __  __/ /__________ _________
 *            \__ \/ / __ `__ \/ / / / __/ ___/ __ `/ ___/ _ \
 *           ___/ / / / / / / / /_/ / /_/ /  / /_/ / /__/  __/
 *          /____/_/_/ /_/ /_/\__,_/\__/_/   \__,_/\___/\___/
 *                         http://simutrace.org
 *
 * Simutrace .Net Binding (SimuTrace.Net.Interop) is part of Simutrace.
 *
 * SimuTrace.Net.Interop is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SimuTrace.Net.Interop is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with SimuTrace.Net.Interop. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Runtime.InteropServices;

namespace SimuTrace
{
    public static partial class Native
    {
        /* SimuBase Types */

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct _Guid
        {
            public ulong ldata;
            public ulong hdata;

            public _Guid(Guid guid)
            {
                byte[] bytes = guid.ToByteArray();

                ldata = (ulong)bytes[0] |
                        ((ulong)bytes[1] << 8) |
                        ((ulong)bytes[2] << 16) |
                        ((ulong)bytes[3] << 24) |
                        ((ulong)bytes[4] << 32) |
                        ((ulong)bytes[5] << 40) |
                        ((ulong)bytes[6] << 48) |
                        ((ulong)bytes[7] << 56);
                hdata = (ulong)bytes[8] |
                        ((ulong)bytes[9] << 8) |
                        ((ulong)bytes[10] << 16) |
                        ((ulong)bytes[11] << 24) |
                        ((ulong)bytes[12] << 32) |
                        ((ulong)bytes[13] << 40) |
                        ((ulong)bytes[14] << 48) |
                        ((ulong)bytes[15] << 56);
            }

            public Guid ToGuid()
            {
                return new Guid(new byte[16] {
                     (byte)(ldata),
                     (byte)(ldata >> 8),
                     (byte)(ldata >> 16),
                     (byte)(ldata >> 24),
                     (byte)(ldata >> 32),
                     (byte)(ldata >> 40),
                     (byte)(ldata >> 48),
                     (byte)(ldata >> 56),
                     (byte)(hdata),
                     (byte)(hdata >> 8),
                     (byte)(hdata >> 16),
                     (byte)(hdata >> 24),
                     (byte)(hdata >> 32),
                     (byte)(hdata >> 40),
                     (byte)(hdata >> 48),
                     (byte)(hdata >> 56),
                    });
            }
        }

        public enum ExceptionClass : uint
        {
            EcUnknown = 0x000,
            EcRuntime = 0x001,
            EcPlatform = 0x002,
            EcPlatformNetwork = 0x003,
            EcNetwork = 0x004,
            EcUser = 0x005
        }

        public enum ExceptionSite : uint
        {
            EsUnknown = 0x000,
            EsClient = 0x001,
            EsServer = 0x002
        }


        /* SimuStor Types */

        public const uint INVALID_SESSION_ID = 0xffffffff;
        public const uint INVALID_STREAM_ID = 0xffffffff;

        public const ulong INVALID_CYCLE_COUNT = 0xffffffffffffffff;
        public const ulong INVALID_ENTRY_INDEX = 0xffffffffffffffff;
        public const ulong INVALID_TIME_STAMP = 0xffffffffffffffff;

        public const uint MAX_STREAM_NAME_LENGTH = 256;
        public const uint MAX_STREAM_TYPE_NAME_LENGTH = 256;
        public const uint TEMPORAL_ORDER_CYCLE_COUNT_BITS = 48;
        public const ulong TEMPORAL_ORDER_CYCLE_COUNT_MASK = ((1UL << 48) - 1);

        [Flags]
        public enum StreamTypeFlags : uint
        {
            StfNone = 0x00,
            StfTemporalOrder = 0x01,
            StfBigEndian = 0x02,
            StfArch23Bit = 0x04
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct StreamTypeDescriptor
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            private char[] _name;
            public string name
            {
                get { return new String(_name, 0, new String(_name).IndexOf('\0')); }
                set
                {
                    value = value + '\0';
                    int l = Math.Min(value.Length, (int)MAX_STREAM_TYPE_NAME_LENGTH);
                    _name = value.ToCharArray(0, l);
                }
            }

            private _Guid _id;
            public Guid id
            {
                get { return _id.ToGuid(); }
                set { _id = new _Guid(value); }
            }

            public StreamTypeFlags flags;

            private uint _reserved1;

            public uint entrySize;

            private uint _reserved2;
        }

        [Flags]
        public enum StreamFlags : uint
        {
            SfNone = 0x00,
            SfHidden = 0x01,
            SfDynamic = 0x02
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct StreamDescriptor
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            private char[] _name;
            public string name
            {
                get { return new String(_name, 0, new String(_name).IndexOf('\0')); }
                set
                {
                    value = value + '\0';
                    int l = Math.Min(value.Length, (int)MAX_STREAM_NAME_LENGTH);
                    _name = value.ToCharArray(0, l);
                }
            }

            public StreamFlags flags;

            private uint _reserved1;

            public StreamTypeDescriptor type;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct StreamRangeInformation
        {
            public ulong startIndex;
            public ulong endIndex;

            public ulong startCycle;
            public ulong endCycle;

            public ulong startTime;
            public ulong endTime;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct StreamStatistics
        {
            public ulong compressedSize;
            public ulong entryCount;
            public ulong rawEntryCount;

            public StreamRangeInformation ranges;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct StreamQueryInformation
        {
            public StreamDescriptor descriptor;
            public StreamStatistics stats;
        }

        public enum QueryIndexType
        {
            QIndex = 0x00,
            QCycleCount = 0x01,
            QRealTime = 0x02,

            QSequenceNumber = QRealTime + 0x01,
            QNextValidSequenceNumber = QRealTime + 0x02,
            QPreviousValidSequenceNumber = QRealTime + 0x03,

            QUserIndex0 = QRealTime + 0x04,
            QUserIndex1 = QRealTime + 0x05,
            QUserIndex2 = QRealTime + 0x06,
            QUserIndex3 = QRealTime + 0x07,
            QUserIndex4 = QRealTime + 0x08,
        }

        [Flags]
        public enum StreamAccessFlags
        {
            SafNone = 0x000,
            SafSequentialScan = 0x001,
            SafRandomAccess = 0x002,
            SafSynchronous = 0x004,
            SafReverseQuery = 0x008,
            SafReverseRead = 0x010,

            SafUserFlag0 = 0x020,
            SafUserFlag1 = 0x040,
            SafUserFlag2 = 0x080,
            SafUserFlag3 = 0x100
        }

        private const uint VARIABLE_ENTRY_SIZE_FLAG = 0x80000000;


        /// <summary>
        /// Variable-sized data streams are distinguished by fixed-sized data
        /// streams by specifically encoding the entry size property of a
        /// stream. To register a stream for variable-sized data use this
        /// method to make the entry size value.
        /// </summary>
        /// <param name="sizeHint">Variable-sized data is stored by splitting
        ///     it into specifically encoded fixed-sized blocks. The size hint
        ///     should be chosen so that it is the expected mean length of the
        ///     variable-sized data that should be stored in the stream.</param>
        /// <returns>The encoded entry size value, which can be used to
        ///     register a new stream.</returns>
        public static uint MakeVariableEntrySize(uint sizeHint)
        {
            const uint VARIABLE_ENTRY_MAX_SIZE =
                ((1 << 14) - 1 + sizeof(ushort));

            if (sizeHint > VARIABLE_ENTRY_MAX_SIZE) {
                sizeHint = VARIABLE_ENTRY_MAX_SIZE;
            }

            return (sizeHint | VARIABLE_ENTRY_SIZE_FLAG);

        }


        /// <summary>
        /// This method determines if the given entry size encodes a
        /// variable entry size.
        /// </summary>
        /// <param name="entrySize">The entry size to check.</param>
        /// <returns>true if the entry size is variable,
        ///     false otherwise.</returns>
        public static bool IsVariableEntrySize(uint entrySize)
        {
            return ((entrySize & VARIABLE_ENTRY_SIZE_FLAG) != 0);
        }


        /// <summary>
        /// This method extracts the entry size of the supplied stream type.
        /// For variable-sized stream types it will return the size hint as
        /// given to MakeVariableEntrySize().
        /// </summary>
        /// <param name="desc">stream type description from which to read
        ///     the entry size.</param>
        /// <returns>The entry size of the given stream type.</returns>
        public static uint GetEntrySize(ref StreamTypeDescriptor desc)
        {
            return desc.entrySize & (~VARIABLE_ENTRY_SIZE_FLAG);
        }


        /* SimuTrace Types */

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct ExceptionInformation
        {
            public ExceptionClass type;
            public ExceptionSite site;

            public int code;

            private uint _reserved;

            private IntPtr _message;
            public string message
            {
                get { return Marshal.PtrToStringAnsi(_message); }
            }
        }

        public delegate int DynamicStreamInitialize(
            ref DynamicStreamDescriptor descriptor, uint id,
            ref IntPtr userData);
        public delegate void DynamicStreamFinalize(uint id, IntPtr userData);

        public delegate int DynamicStreamOpen(
            ref DynamicStreamDescriptor descriptor, uint id,
            QueryIndexType type, ulong value, StreamAccessFlags flags,
            out IntPtr userDataOut);
        public delegate void DynamicStreamClose(uint id, ref IntPtr userData);

        public delegate int DynamicStreamGetNextEntry(IntPtr userData,
            out IntPtr entryOut);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DynamicStreamOperations
        {
            public DynamicStreamInitialize initialize;
            public DynamicStreamFinalize findalize;

            public DynamicStreamOpen open;
            public DynamicStreamClose close;

            public DynamicStreamGetNextEntry getNextEntry;

            private IntPtr _reserved0;
            private IntPtr _reserved1;
            private IntPtr _reserved2;
            private IntPtr _reserved3;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DynamicStreamDescriptor
        {
            public StreamDescriptor @base;
            public DynamicStreamOperations operations;

            public IntPtr userData;

            private ulong _reserved0;
            private ulong _reserved1;
            private ulong _reserved2;
            private ulong _reserved3;
        }


        /* SimuTraceEntryTypes Types */

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Data32
        {
            [FieldOffset(0x00)]
            public uint data32;

            [FieldOffset(0x00)]
            public byte data8;

            [FieldOffset(0x00)]
            public ushort data16;

            [FieldOffset(0x02)]
            public ushort size;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Data64
        {
            [FieldOffset(0x00)]
            public ulong data64;

            [FieldOffset(0x00)]
            public byte data8;

            [FieldOffset(0x00)]
            public ushort data16;

            [FieldOffset(0x00)]
            public uint data32;

            [FieldOffset(0x04)]
            public uint size;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct MemoryAccessMetaData
        {
            [FieldOffset(0x00)]
            public ulong cycleCount;

            [FieldOffset(0x06)]
            public ushort flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DataMemoryAccess32
        {
            public MemoryAccessMetaData metadata;
            public uint ip;
            public uint address;
            public Data32 data;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MemoryAccess32
        {
            public MemoryAccessMetaData metadata;
            public uint ip;
            public uint address;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DataMemoryAccess64
        {
            public MemoryAccessMetaData metadata;
            public ulong ip;
            public ulong address;
            public Data64 data;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MemoryAccess64
        {
            public MemoryAccessMetaData metadata;
            public ulong ip;
            public ulong address;
        }

        public enum ArchitectureSize : uint
        {
            As32Bit = 0x00,
            As64Bit = 0x01
        }

        public enum MemoryAccessType : uint
        {
            MatRead  = 0x00,
            MatWrite = 0x01
        }

        public enum MemoryAddressType : uint
        {
            AtPhysical = 0x00,
            AtVirtual  = 0x01,

            AtLogical  = AtVirtual,
            AtLinear   = AtVirtual
        }


        /* Base API */

        /// <summary>
        /// Version number of the Simutrace client. This should ideally match
        /// the version of the server. The version number has the form:
        /// <Revision (16 bits), Major Version (8 bits), Minor Version (8 bits)>
        /// with the revision being stored in the high word.
        /// </summary>
        /// <returns></returns>
        public static uint StGetClientVersion()
        {
            return NativeMethods.StGetClientVersion();
        }


        /// <summary>
        /// Returns extended exception information on the last error during a
        /// Simutrace API call.
        /// </summary>
        /// <param name="informationOut">An ExceptionInformation structure
        ///     that will receive the error information.</param>
        /// <returns>The last error code. For platform exceptions this is the
        ///     operating system error code.</returns>
        public static int StGetLastError(
            out ExceptionInformation informationOut)
        {
            return NativeMethods.StGetLastError(out informationOut);
        }


        /// <summary>
        /// Returns the last error during a Simutrace API call.
        /// </summary>
        /// <returns>The last error code. For platform exceptions this is the
        ///     operating system error code.</returns>
        public static int StGetLastError()
        {
            ExceptionInformation info;
            return StGetLastError(out info);
        }


        /// <summary>
        /// Sets the last error information for the calling thread. The value
        /// can be retrieved with StGetLastError().
        /// </summary>
        /// <param name="type">The type of the exception, which determines how
        ///     to interpret the error code. Set this to EcUser for
        ///     user-defined error codes.</param>
        /// <param name="code">One of the pre-defined error codes or a
        ///     user-defined one, depending on the exception type.</param>
        /// <param name="message">Optional pointer to an error message. This
        ///     parameter can be \c null.</param>
        public static void StSetLastError(ExceptionClass type, int code,
            string message)
        {
            NativeMethods.StSetLastError(type, code, message);
        }


        /* Session API */

        /// <summary>
        /// Connections of a client to a Simutrace storage server are
        /// represented as a session. This method connects to the specified
        /// server and creates a new local session as well as a new session on
        /// the server side. Calling this function is the first step, when
        /// working with Simutrace. To successfully establish a connection,
        /// the server process must already be running and accept connections
        /// on the address specified by @p server.
        /// </summary>
        /// <param name="server">Specifies the connection string used to
        ///     contact the server. The specifier must be supplied in the form
        ///     channel:address. For more information please see the C/C++
        ///     documentation.</param>
        /// <returns>A session id, identifying the newly created session if
        ///     successful, INVALID_SESSION_ID otherwise. For a more detailed
        ///     error description call StGetLastError().</returns>
        public static uint StSessionCreate(string server)
        {
            return NativeMethods.StSessionCreate(server);
        }


        /// <summary>
        /// Each thread that wants to participate in a tracing session (e.g.,
        /// read or write data via the tracing API), must be connected to a
        /// session. This method connects a thread to an existing local session.
        /// </summary>
        /// <param name="session">The id of the session to be opened as
        ///     originally returned by StSessionCreate().</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StSessionOpen(uint session)
        {
            return NativeMethods.StSessionOpen(session);
        }


        /// <summary>
        /// This method closes a thread's reference to an open session. If
        /// this is the last reference, the session is run down. After closing
        /// the session, the executing thread may connect to a different
        /// session again.
        /// </summary>
        /// <param name="session">The id of the session to be closed as
        ///     originally returned by StSessionCreate().</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StSessionClose(uint session)
        {
            return NativeMethods.StSessionClose(session);
        }


        /// <summary>
        /// A store is the data back-end for a Simutrace tracing session. All
        /// trace data recorded or read in a session is stored in the
        /// currently open store. Creating or opening a store is thus
        /// typically the next operation after creating a new session.
        /// This method creates a new store.
        /// </summary>
        /// <param name="session">The id of the session to open a store in
        ///     as originally returned by StSessionCreate().</param>
        /// <param name="specifier">Simutrace comes with a modular storage
        ///     back-end that can employ custom trace formats to save trace
        ///     data. The specifier must be supplied in the form format:path.
        ///     For more information please see the C/C++ documentation.</param>
        /// <param name="alwaysCreate">Indicates if the store should be
        ///     overwritten, if it already exists.</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StSessionCreateStore(uint session,
            string specifier, bool alwaysCreate)
        {
            return NativeMethods.StSessionCreateStore(session, specifier,
                alwaysCreate);
        }


        /// <summary>
        /// A store is the data back-end for a Simutrace tracing session. All
        /// trace data recorded or read in a session is stored in the currently
        /// open store. Creating or opening a store is thus typically the next
        /// operation after creating a new session. This method opens an
        /// existing store.
        /// </summary>
        /// <param name="session">The id of the session to open a store in
        ///     as originally returned by StSessionCreate().</param>
        /// <param name="specifier">Simutrace comes with a modular storage
        ///     back-end that can employ custom trace formats to save trace
        ///     data. The specifier must be supplied in the form format:path.
        ///     For more information on storage specifier please refer to
        ///     StSessionCreateStore().</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StSessionOpenStore(uint session, string specifier)
        {
            return NativeMethods.StSessionOpenStore(session, specifier);
        }


        /// <summary>
        /// Each session may have only one store open at a time. Use this
        /// function to close the currently open store. This allows you to
        /// switch to a different store without creating a new session.
        /// </summary>
        /// <param name="session">The id of the session whose store should be
        ///     closed as originally returned by StSessionCreate().</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StSessionCloseStore(uint session)
        {
            return NativeMethods.StSessionCloseStore(session);
        }


        /// <summary>
        /// This method allows configuring various parameters for the specified
        /// session. For a reference of all configurable parameters see the
        /// sample server configuration file, which comes with Simutrace.
        /// </summary>
        /// <param name="session">The id of the session to be configured as
        ///     originally returned by StSessionCreate().</param>
        /// <param name="configuration">A string describing the configuration
        ///     to set. For information on the format see the documentation of
        ///     libconfig.</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StSessionSetConfiguration(uint session,
            string configuration)
        {
            return NativeMethods.StSessionSetConfiguration(session,
                configuration);
        }


        /* Stream API */

        /// <summary>
        /// This method is a helper function to quickly create a new stream
        /// description needed to register a new stream. The description
        /// contains information about the stream's name and layout of trace
        /// entries, that is records written to or read from a stream.
        /// </summary>
        /// <param name="name">A friendly name of the new stream (e.g.,
        ///     "CPU Memory Accesses")</param>
        /// <param name="entrySize">The size of a single trace entry in bytes.
        ///     To specify a variable-sized entry type, use
        ///     makeVariableEntrySize().</param>
        /// <param name="flags">Supplies the flags used for the type of the new
        ///     stream.</param>
        /// <param name="descOut">StreamDescriptor structure that will
        ///     receive the new stream information.</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StMakeStreamDescriptor(string name,
            uint entrySize, StreamTypeFlags flags, out StreamDescriptor descOut)
        {
            return NativeMethods.StMakeStreamDescriptor(name, entrySize,
                flags, out descOut);
        }


        /// <summary>
        /// This method is a helper function to quickly create a new stream
        /// description needed to register a new stream. The description
        /// contains information about the stream's name and layout of trace
        /// entries, that is records written to or read from a stream. This
        /// method uses the supplied type information.
        /// </summary>
        /// <param name="name">A friendly name of the new stream (e.g.,
        ///     "CPU Memory Accesses").</param>
        /// <param name="type">Reference to a valid StreamTypeDescriptor
        ///     structure, defining the desired type of the new stream.</param>
        /// <param name="descOut">StreamDescriptor structure that will
        ///     receive the new stream information.</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StMakeStreamDescriptorFromType(string name,
            ref StreamTypeDescriptor type, out StreamDescriptor descOut)
        {
            return NativeMethods.StMakeStreamDescriptorFromType(name, ref type,
                out descOut);
        }


        /// <summary>
        /// Simutrace comes with an integrated aggressive, but fast
        /// compressor for memory traces. To utilize the compressor, the
        /// stream must use one of the types returned by this method and
        /// employ the \c simtrace storage format (see StSessionCreateStore()).
        /// </summary>
        /// <param name="size">Architecture size of the simulated
        ///     system.</param>
        /// <param name="accessType">Type of memory operations that the new
        ///     stream should hold.</param>
        /// <param name="addressType">The semantic of the address
        ///     field.</param>
        /// <param name="hasData">Indicates if the memory entry will contain
        ///     the data read or written with the memory entry.</param>
        /// <returns>Stream descriptor identifying the desired memory entry
        ///     type. Do not modify the returned stream descriptor.</returns>
        public static StreamTypeDescriptor StStreamFindMemoryType(
            ArchitectureSize size, MemoryAccessType accessType,
            MemoryAddressType addressType, bool hasData)
        {
            return NativeMethods.StStreamFindMemoryType(size, accessType,
                addressType, hasData);
        }


        /// <summary>
        /// This method is a helper function to quickly create dynamic stream
        /// descriptions needed to register a new dynamic stream. The
        /// descriptions contains information about the stream's name, layout,
        /// and operations.
        /// </summary>
        /// <param name="name">A friendly name of the new stream (e.g.,
        ///     "Filtered memory accesses").</param>
        /// <param name="entrySize">The size of a single trace entry in bytes.
        ///     Variable-sized entries are not supported.</param>
        /// <param name="flags">Supplies the flags used for the type of the
        ///     new stream.</param>
        /// <param name="userData">A pointer to optional user-defined data that
        ///     should be available when the stream is opened. The data must
        ///     not be freed for the lifetime of the stream.</param>
        /// <param name="operations">Pointer to a set of operations that
        ///     implement the logic of the dynamic stream.</param>
        /// <param name="descOut">Pointer to a DynamicStreamDescriptor
        ///     structure that will receive the new stream information.</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StMakeStreamDescriptorDynamic(string name,
            uint entrySize, StreamTypeFlags flags, IntPtr userData,
            ref DynamicStreamOperations operations,
            out DynamicStreamDescriptor descOut)
        {
            return NativeMethods.StMakeStreamDescriptorDynamic(name,
                entrySize, flags, userData, ref operations, out descOut);
        }


        /// <summary>
        /// This method is a helper function to quickly create dynamic stream
        /// descriptions needed to register a new dynamic stream. The
        /// descriptions contains information about the stream's name, layout,
        /// and operations. This method uses the supplied type information.
        /// </summary>
        /// <param name="name">A friendly name of the new stream (e.g.,
        ///     "Filtered memory accesses").</param>
        /// <param name="userData">Pointer to a valid StreamTypeDescriptor
        ///     structure, defining the desired type of the new stream.</param>
        /// <param name="type">A pointer to optional user-defined data that
        ///     should be available when the stream is opened. The data
        ///     must not be freed for the lifetime of the stream.</param>
        /// <param name="operations">Pointer to a set of operations that
        ///     implement the logic of the dynamic stream.</param>
        /// <param name="descOut">Pointer to a DynamicStreamDescriptor
        ///     structure that will</param>
        /// <returns>true if successful, false otherwise. For a more detailed
        ///     error description call StGetLastError().</returns>
        public static bool StMakeStreamDescriptorDynamicFromType(string name,
            IntPtr userData, ref StreamTypeDescriptor type,
            ref DynamicStreamOperations operations,
            out DynamicStreamDescriptor descOut)
        {
            return NativeMethods.StMakeStreamDescriptorDynamicFromType(name,
                userData, ref type, ref operations, out descOut);
        }


        /// <summary>
        /// Streams are the basic interface to write or read data with
        /// Simutrace. Streams always belong to a store. This method registers
        /// a new stream in the session's active store. Before doing so, the
        /// caller must initially create a new store with StSessionCreateStore().
        /// </summary>
        /// <param name="session">The id of the session, whose store should
        ///     register the stream.</param>
        /// <param name="desc">Reference to a stream descriptor defining the
        ///     properties of the new stream (e.g., the desired type of data
        ///     entries). To create a descriptor see StMakeStreamDescriptor()
        ///     or StMakeStreamDescriptorFromType().</param>
        /// <returns>The id of the new stream if successful, INVALID_STREAM_ID
        ///     otherwise. For a more detailed error description call
        ///     StGetLastError().</returns>
        public static uint StStreamRegister(uint session,
            ref StreamDescriptor desc)
        {
            return NativeMethods.StStreamRegister(session, ref desc);
        }


        /// <summary>
        /// Streams are the basic interface to work with data in Simutrace.
        /// Dynamic streams differ from regular streams in that entries are
        /// created dynamically by user-defined handler functions. This way
        /// multiplexers, filters, etc. can be realized. A filter, for example,
        /// may connect to a regular static stream and pass only those entries
        /// that meet certain criteria. Accessing the resulting dynamic stream
        /// does not differ from regular streams. Dynamic streams therefore may
        /// be used as input for other dynamic streams, building a cascade of
        /// stream processors.
        /// </summary>
        /// <param name="session">The id of the session, whose store should
        ///     register the stream.</param>
        /// <param name="desc">Pointer to a dynamic stream descriptor defining
        ///     the properties of the new dynamic stream (e.g., the desired
        ///     type of data entries). To create a dynamic descriptor see
        ///     StMakeStreamDescriptorDynamic() or
        ///     StMakeStreamDescriptorDynamicFromType().</param>
        /// <returns>The id of the new dynamic stream if successful,
        ///     INVALID_STREAM_ID otherwise. For a more detailed error
        ///     description call StGetLastError().</returns>
        public static uint StStreamRegisterDynamic(uint session,
            ref DynamicStreamDescriptor desc)
        {
            return NativeMethods.StStreamRegisterDynamic(session, ref desc);
        }


        /// <summary>
        /// After registering streams or opening an existing store, all streams
        /// are available through their corresponding ids. This method
        /// enumerates all registered streams by returning a list of valid
        /// stream ids. To get information on a certain stream call
        /// StStreamQuery().
        /// </summary>
        /// <param name="session">The id of the session to enumerate all
        ///     streams from.</param>
        /// <param name="streamIdsOut">The array will hold the list of stream
        ///     ids.</param>
        /// <returns>The number of valid stream ids, that is registered
        ///     streams. The method will return -1 on error. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static int StStreamEnumerate(uint session,
            out uint[] streamIdsOut)
        {
            int num = NativeMethods.StStreamEnumerate(session, IntPtr.Zero,
                IntPtr.Zero);
            if (num <= 0) {
                streamIdsOut = null;
                return num;
            }

            streamIdsOut = new uint[num];

            GCHandle pids = new GCHandle();
            try {
                pids = GCHandle.Alloc(streamIdsOut, GCHandleType.Pinned);

                IntPtr len = (IntPtr)(streamIdsOut.Length * sizeof(uint));
                num = NativeMethods.StStreamEnumerate(session, len,
                    pids.AddrOfPinnedObject());
                if (num == -1) {
                    streamIdsOut = null;
                }
            } finally {
                if (pids.IsAllocated) {
                    pids.Free();
                }
            }

            return num;
        }


        /// <summary>
        /// Returns the number of streams in a session.
        /// </summary>
        /// <param name="session">The id of the session to get the number of
        ///     streams from.</param>
        /// <returns>The number of valid stream ids, that is registered
        ///     streams. The method will return -1 on error. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static int StStreamEnumerate(uint session)
        {
            return NativeMethods.StStreamEnumerate(session, IntPtr.Zero,
                IntPtr.Zero);
        }


        /// <summary>
        /// This method returns detailed information on the properties of a
        /// stream. The information includes the #StreamDescriptor originally
        /// supplied to create the stream as well as a set of statistics such
        /// as the compressed size of the stream. See #StreamQueryInformation
        /// for furthers details.
        /// </summary>
        /// <param name="session">The id of the session that holds the
        ///     stream of interest.</param>
        /// <param name="stream">The id of the stream to query.</param>
        /// <param name="informationOut">A reference to a
        ///     StreamQueryInformation structure which will receive the
        ///     stream information.</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StStreamQuery(uint session, uint stream,
            out StreamQueryInformation informationOut)
        {
            return NativeMethods.StStreamQuery(session, stream,
                out informationOut);
        }


        /// <summary>
        /// This method opens a write handle to the specified stream.
        /// The handle can be used with StGetNextEntryFast() to append new
        /// entries to a stream.
        /// </summary>
        /// <param name="session">The id of the session that holds the
        ///     stream of interest.</param>
        /// <param name="stream">The id of the stream to open a
        ///     handle for.</param>
        /// <returns>A write handle to the specified stream if successful,
        ///     0 otherwise. For a more detailed error description call
        ///     StGetLastError().</returns>
        public static IntPtr StStreamAppend(uint session, uint stream)
        {
            return NativeMethods.StStreamAppend(session, stream, IntPtr.Zero);
        }


        /// <summary>
        /// This method opens a new read handle to the specified stream. The
        /// handle can be used with StGetNextEntryFast() and
        /// StGetPreviousEntryFast() to read entries.
        /// </summary>
        /// <param name="session">The id of the session that holds the
        ///     stream of interest.</param>
        /// <param name="stream">The id of the stream to open a
        ///     handle for.</param>
        /// <param name="type">Type of property value that indicates which
        ///     position in the stream should be opened. For further
        ///     information on the possible query types
        ///     see QueryIndexType.</param>
        /// <param name="value">A value used in the query specified by type.
        ///     See QueryIndexType for more details.</param>
        /// <param name="flags">Supplies information on how the caller intends
        ///     to access the stream with the requested handle. For more
        ///     information see StreamAccessFlags.</param>
        /// <returns>A read handle to the specified stream if successful,
        ///     0 otherwise. For a more detailed error description call
        ///     StGetLastError().</returns>
        public static IntPtr StStreamOpen(uint session, uint stream,
            QueryIndexType type, ulong value, StreamAccessFlags flags)
        {
            return NativeMethods.StStreamOpen(session, stream, type, value,
                flags, IntPtr.Zero);
        }


        /// <summary>
        /// This method closes the supplied stream handle.
        /// </summary>
        /// <param name="handle">The handle to be closed.</param>
        /// <returns>true if successful, false otherwise. For a more
        ///     detailed error description call StGetLastError().</returns>
        public static bool StStreamClose(IntPtr handle)
        {
            return NativeMethods.StStreamClose(handle);
        }


        /* Tracing API */

        /// <summary>
        /// This method moves the read/write pointer of the supplied handle to
        /// the next entry in the stream. The pointer returned by this method
        /// can be used to write or read (depending on the handle) a single
        /// trace entry.
        /// </summary>
        /// <param name="handle">The stream handle. The method may allocate a
        ///     new handle and update the supplied one.</param>
        /// <returns>A pointer to the next trace entry if successful, 0
        ///     otherwise. For a more detailed error description call
        ///     StGetLastError().</returns>
        public static IntPtr StGetNextEntry(ref IntPtr handle)
        {
            return NativeMethods.StGetNextEntry(ref handle);
        }


        /// <summary>
        /// This method moves the read/write pointer of the supplied handle to
        /// the previous entry in the stream. The pointer returned by this
        /// method can only be used to read a single trace entry.
        /// </summary>
        /// <param name="handle">A stream read handle. The
        ///     method may allocate a new handle and update the supplied
        ///     one.</param>
        /// <returns>A pointer to the previous trace entry if successful, 0
        ///     otherwise. For a more detailed error description call
        ///     StGetLastError(). On error, the supplied handle will be
        ///     invalidated. In contrast to a closed handle
        ///     (see StStreamClose()), an invalidated handle is not freed and
        ///     may be used to retry the operation. If successful, the caller
        ///     must cast the pointer to the data type (e.g., #DataWrite64)
        ///     for the type supplied at stream registration.</returns>
        public static IntPtr StGetPreviousEntry(ref IntPtr handle)
        {
            return NativeMethods.StGetPreviousEntry(ref handle);
        }


        /// <summary>
        /// This method builds the closing operation when writing a new entry
        /// to a stream, signaling Simutrace that the entry has been fully
        /// written and is valid.
        /// </summary>
        /// <param name="handle">The write handle of the stream, which has been
        ///     used in matching call to StGetNextEntryFast().</param>
        public static void StSubmitEntry(IntPtr handle)
        {
            NativeMethods.StSubmitEntry(handle);
        }


        /// <summary>
        /// In Simutrace, entries are required to be of fixed size. This
        /// method enables tracing of variable-sized data such as strings. To
        /// trace entries that contain variable-sized fields, the data fields
        /// should be replaced with fixed-size reference fields. The data
        /// itself can then be traced with this method.
        /// </summary>
        /// <param name="handle">Handle for the stream that will receive the
        ///     variable-sized data.</param>
        /// <param name="sourceBuffer">Pointer to a buffer containing the
        ///     variable-sized data.</param>
        /// <param name="sourceLength">Length of the variable-sized data.</param>
        /// <param name="referenceOut">Reference field receiving the reference
        ///     to the variable-sized data in the given stream.</param>
        /// <returns>The number of bytes written, \p sourceLength if successful.
        ///     For a more detailed error description call
        ///     StGetLastError().</returns>
        public static IntPtr StWriteVariableData(ref IntPtr handle,
            IntPtr sourceBuffer, IntPtr sourceLength, out ulong referenceOut)
        {
            return NativeMethods.StWriteVariableData(ref handle, sourceBuffer,
                sourceLength, out referenceOut);
        }


        /// <summary>
        /// In Simutrace, entries are required to be of fixed size. This
        /// method enables tracing of variable-sized data such as strings. To
        /// trace entries that contain variable-sized fields, the data fields
        /// should be replaced with fixed-size reference fields. The data
        /// itself can then be traced with this method.
        /// </summary>
        /// <param name="handle">Handle for the stream that will receive the
        ///     variable-sized data.</param>
        /// <param name="sourceBuffer">Pointer to a buffer containing the
        ///     variable-sized data.</param>
        /// <param name="sourceLength">Length of the variable-sized data.</param>
        /// <returns>The number of bytes written, \p sourceLength if successful.
        ///     For a more detailed error description call
        ///     StGetLastError().</returns>
        private static IntPtr StWriteVariableData(ref IntPtr handle,
            IntPtr sourceBuffer, IntPtr sourceLength)
        {
            ulong reference;

            return NativeMethods.StWriteVariableData(ref handle, sourceBuffer,
                sourceLength, out reference);
        }


        /// <summary>
        /// This method reads variable-sized data from the supplied stream
        /// given a reference, originally returned by StWriteVariableData().
        /// </summary>
        /// <param name="handle">Reference to a read handle for the stream
        ///     that stores the variable-sized data.</param>
        /// <param name="reference">Reference to the variable-sized data in
        ///     the given stream.</param>
        /// <param name="destinationBuffer">Pointer to a buffer receiving the
        ///     variable-sized data. The buffer needs provide enough space.
        ///     Supply IntPtr.Zero to determine the length of the data.</param>
        /// <returns>The length of the referenced variable-sized data in bytes,
        ///     if successful, -1 otherwise. For a more detailed error
        ///     description call StGetLastError().</returns>
        public static IntPtr StReadVariableData(ref IntPtr handle,
            ulong reference, IntPtr destinationBuffer)
        {
            return NativeMethods.StReadVariableData(ref handle, reference,
                destinationBuffer);
        }

    }
}