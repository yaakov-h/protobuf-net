﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.IO.Pipelines
{
    public class StreamPipeConnection : IPipeConnection
    {
        public StreamPipeConnection(PipeOptions options, Stream stream)
        {
            Input = CreateReader(options, stream);
            Output = CreateWriter(options, stream);
        }

        public IPipeReader Input { get; }

        public IPipeWriter Output { get; }

        public void Dispose()
        {
            Input.Complete();
            Output.Complete();
        }

        public static IPipeReader CreateReader(PipeOptions options, Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new NotSupportedException();
            }

            var pipe = new Pipe(options);
            var ignore = stream.CopyToEndAsync(pipe.Writer);

            return pipe.Reader;
        }

        public static IPipeWriter CreateWriter(PipeOptions options, Stream stream)
        {
            var pipe = new Pipe(options);
            if (stream.CanWrite)
            {
                var _ = pipe.Reader.CopyToEndAsync(stream);
            }
            else
            {
                pipe.Writer.Complete();
            }
            return pipe.Writer;
        }
    }
}