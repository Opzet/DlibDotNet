﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DlibDotNet.Extensions;

namespace DlibDotNet.Dnn
{

    /// <summary>
    /// Represents a loss layer for a deep neural network for the multiclass logistic regression loss (e.g. negative log-likelihood loss), which is appropriate for multiclass classification problems. In particular, this class handles matrix outputs. This class cannot be inherited.
    /// </summary>
    public sealed class LossMulticlassLogPerPixel : Net
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LossMulticlassLogPerPixel"/> class with a specified network type of deep neural network.
        /// </summary>
        /// <param name="networkType">The network type.</param>
        public LossMulticlassLogPerPixel(int networkType = 0)
            : base(networkType)
        {
            var ret = NativeMethods.LossMulticlassLogPerPixel_new(networkType, out var net);
            if (ret == NativeMethods.ErrorType.DnnNotSupportNetworkType)
                throw new NotSupportNetworkTypeException(networkType);

            this.NativePtr = net;
        }

        internal LossMulticlassLogPerPixel(IntPtr ptr, int networkType = 0, bool isEnabledDispose = true)
            : base(networkType, isEnabledDispose)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException("Can not pass IntPtr.Zero", nameof(ptr));

            this.NativePtr = ptr;
        }

        #endregion

        #region Properties

        public static ushort LabelToIgnore => NativeMethods.LossMulticlassLogPerPixel_get_label_to_ignore();

        public override int NumLayers
        {
            get
            {
                this.ThrowIfDisposed();

                return NativeMethods.LossMulticlassLogPerPixel_get_num_layers(this.NetworkType);
            }
        }

        #endregion

        #region Methods

        public override void Clean()
        {
            this.ThrowIfDisposed();

            NativeMethods.LossMulticlassLogPerPixel_clean(this.NetworkType, this.NativePtr);
        }

        public LossMulticlassLogPerPixel CloneAs(int networkType)
        {
            this.ThrowIfDisposed();

            var ret = NativeMethods.LossMulticlassLogPerPixel_cloneAs(this.NetworkType, this.NativePtr, networkType, out var net);
            if (ret == NativeMethods.ErrorType.DnnNotSupportNetworkType)
                throw new NotSupportNetworkTypeException(networkType);

            return new LossMulticlassLogPerPixel(net, networkType);
        }

        public static LossMulticlassLogPerPixel Deserialize(string path, int networkType = 0)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} is not found", path);

            var str = Dlib.Encoding.GetBytes(path);
            var error = NativeMethods.LossMulticlassLogPerPixel_deserialize(networkType, 
                                                                            str,
                                                                            str.Length,
                                                                            out var net,
                                                                            out var errorMessage);
            Cuda.ThrowCudaException(error);
            switch (error)
            {
                case NativeMethods.ErrorType.DnnNotSupportNetworkType:
                    throw new NotSupportNetworkTypeException(networkType);
                case NativeMethods.ErrorType.GeneralSerialization:
                    throw new SerializationException(StringHelper.FromStdString(errorMessage, true));
            }

            return new LossMulticlassLogPerPixel(net, networkType);
        }

        public static LossMulticlassLogPerPixel Deserialize(byte[] content, int networkType = 0)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var error = NativeMethods.LossMulticlassLogPerPixel_deserialize2(networkType,
                                                                             content,
                                                                             content.Length,
                                                                             out var net,
                                                                             out var errorMessage);
            Cuda.ThrowCudaException(error);
            switch (error)
            {
                case NativeMethods.ErrorType.DnnNotSupportNetworkType:
                    throw new NotSupportNetworkTypeException(networkType);
                case NativeMethods.ErrorType.GeneralSerialization:
                    throw new SerializationException(StringHelper.FromStdString(errorMessage, true));
            }

            return new LossMulticlassLogPerPixel(net, networkType);
        }

        public static LossMulticlassLogPerPixel Deserialize(ProxyDeserialize deserialize, int networkType = 0)
        {
            if (deserialize == null)
                throw new ArgumentNullException(nameof(deserialize));

            deserialize.ThrowIfDisposed();

            var error = NativeMethods.LossMulticlassLogPerPixel_deserialize_proxy(networkType, 
                                                                                  deserialize.NativePtr, 
                                                                                  out var net,
                                                                                  out var errorMessage);
            Cuda.ThrowCudaException(error);
            switch (error)
            {
                case NativeMethods.ErrorType.DnnNotSupportNetworkType:
                    throw new NotSupportNetworkTypeException(networkType);
                case NativeMethods.ErrorType.GeneralSerialization:
                    throw new SerializationException(StringHelper.FromStdString(errorMessage, true));
            }

            return new LossMulticlassLogPerPixel(net, networkType);
        }
        
        public LossDetails GetLossDetails()
        {
            this.ThrowIfDisposed();

            NativeMethods.LossMulticlassLogPerPixel_get_loss_details(this.NetworkType, this.NativePtr, out var lossDetails);
            return new LossDetails(this, lossDetails);
        }

        public Subnet GetSubnet()
        {
            this.ThrowIfDisposed();

            return new Subnet(this);
        }

        internal override DPoint InputTensorToOutputTensor(DPoint p)
        {
            using (var np = p.ToNative())
            {
                NativeMethods.LossMulticlassLogPerPixel_input_tensor_to_output_tensor(this.NetworkType, this.NativePtr, np.NativePtr, out var ret);
                return new DPoint(ret);
            }
        }
        
        internal override void NetToXml(string filename)
        {
            var fileNameByte = Dlib.Encoding.GetBytes(filename);
            NativeMethods.LossMulticlassLogPerPixel_net_to_xml(this.NetworkType, this.NativePtr, fileNameByte, fileNameByte.Length);
        }

        public OutputLabels<Matrix<ushort>> Operator<T>(Matrix<T> image, ulong batchSize = 128)
            where T : struct
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            image.ThrowIfDisposed();

            return this.Operator(new[] { image }, batchSize);
        }

        public OutputLabels<Matrix<ushort>> Operator<T>(IEnumerable<Matrix<T>> images, ulong batchSize = 128)
            where T : struct
        {
            if (images == null)
                throw new ArgumentNullException(nameof(images));
            if (!images.Any())
                throw new ArgumentException();
            if (images.Any(matrix => matrix == null))
                throw new ArgumentException();

            images.ThrowIfDisposed();
            
            Matrix<T>.TryParse<T>(out var imageType);
            var templateRows = images.First().TemplateRows;
            var templateColumns = images.First().TemplateColumns;

            // vecOut is not std::vector<Matrix<float>*>* but std::vector<Matrix<float>>*.
            var array = images.Select(matrix => matrix.NativePtr).ToArray();
            var ret = NativeMethods.LossMulticlassLogPerPixel_operator_matrixs(this.NetworkType,
                                                                               this.NativePtr,
                                                                               imageType.ToNativeMatrixElementType(),
                                                                               array,
                                                                               array.Length,
                                                                               templateRows,
                                                                               templateColumns,
                                                                               (uint)batchSize,
                                                                               out var vecOut);

            Cuda.ThrowCudaException(ret);
            switch (ret)
            {
                case NativeMethods.ErrorType.MatrixElementTypeNotSupport:
                    throw new ArgumentException($"{imageType} is not supported.");
            }

            return new Output(vecOut);
        }

        public static void Serialize(LossMulticlassLogPerPixel net, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException();

            net.ThrowIfDisposed();

            var str = Dlib.Encoding.GetBytes(path);
            var error = NativeMethods.LossMulticlassLogPerPixel_serialize(net.NetworkType, net.NativePtr, str, str.Length, out var errorMessage);
            switch (error)
            {
                case NativeMethods.ErrorType.DnnNotSupportNetworkType:
                    throw new NotSupportNetworkTypeException(net.NetworkType);
                case NativeMethods.ErrorType.GeneralSerialization:
                    throw new SerializationException(StringHelper.FromStdString(errorMessage, true));
            }
        }
        
        public static void Serialize(ProxySerialize serialize, LossMulticlassLogPerPixel net)
        {
            if (serialize == null)
                throw new ArgumentNullException(nameof(serialize));
            if (net == null)
                throw new ArgumentNullException(nameof(net));

            net.ThrowIfDisposed();

            var error = NativeMethods.LossMulticlassLogPerPixel_serialize_proxy(net.NetworkType,
                                                                                serialize.NativePtr,
                                                                                net.NativePtr,
                                                                                out var errorMessage);
            switch (error)
            {
                case NativeMethods.ErrorType.DnnNotSupportNetworkType:
                    throw new NotSupportNetworkTypeException(net.NetworkType);
                case NativeMethods.ErrorType.GeneralSerialization:
                    throw new SerializationException(StringHelper.FromStdString(errorMessage, true));
            }
        }

        public static void TestOneStep<T>(DnnTrainer<LossMulticlassLogPerPixel> trainer, IEnumerable<Matrix<T>> data, IEnumerable<Matrix<ushort>> label)
            where T : struct
        {
            if (trainer == null)
                throw new ArgumentNullException(nameof(trainer));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (label == null)
                throw new ArgumentNullException(nameof(label));
            if (data.Count() != label.Count())
                throw new ArgumentException($"The count of {nameof(data)} must equal to {nameof(label)}'s.");

            Matrix<T>.TryParse<T>(out var dataElementTypes);

            using (var dataVec = new StdVector<Matrix<T>>(data))
            using (var labelVec = new StdVector<Matrix<ushort>>(label))
            {
                var ret = NativeMethods.LossMulticlassLogPerPixel_trainer_test_one_step(trainer.Type,
                                                                                        trainer.NativePtr,
                                                                                        trainer.SolverType,
                                                                                        dataElementTypes.ToNativeMatrixElementType(),
                                                                                        dataVec.NativePtr,
                                                                                        NativeMethods.MatrixElementType.UInt32,
                                                                                        labelVec.NativePtr);
                Cuda.ThrowCudaException(ret);

                switch (ret)
                {
                    case NativeMethods.ErrorType.MatrixElementTypeNotSupport:
                        throw new NotSupportedException($"{dataElementTypes} does not support");
                }
            }
        }

        /// <summary>
        /// Trains a supervised neural network based on the given training data.
        /// </summary>
        /// <typeparam name="T">The type of element in the matrix.</typeparam>
        /// <param name="trainer">The trainer object of <see cref="LossMulticlassLogPerPixel"/>.</param>
        /// <param name="data">The training data.</param>
        /// <param name="label">The label.</param>
        /// <exception cref="ArgumentNullException"><paramref name="trainer"/>, <paramref name="data"/> or <paramref name="label"/> is null.</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="trainer"/> is disposed.</exception>
        /// <exception cref="NotSupportedException">The specified type of element in the matrix does not supported.</exception>
        public static void Train<T>(DnnTrainer<LossMulticlassLogPerPixel> trainer, IEnumerable<Matrix<T>> data, IEnumerable<Matrix<ushort>> label)
            where T : struct
        {
            if (trainer == null)
                throw new ArgumentNullException(nameof(trainer));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (label == null)
                throw new ArgumentNullException(nameof(label));
            if (data.Count() != label.Count())
                throw new ArgumentException($"The count of {nameof(data)} must equal to {nameof(label)}'s.");

            trainer.ThrowIfDisposed();

            Matrix<T>.TryParse<T>(out var dataElementTypes);

            using (var dataVec = new StdVector<Matrix<T>>(data))
            using (var labelVec = new StdVector<Matrix<ushort>>(label))
            {
                var ret = NativeMethods.LossMulticlassLogPerPixel_trainer_train(trainer.Type,
                                                                                trainer.NativePtr,
                                                                                trainer.SolverType,
                                                                                dataElementTypes.ToNativeMatrixElementType(),
                                                                                dataVec.NativePtr,
                                                                                NativeMethods.MatrixElementType.UInt32,
                                                                                labelVec.NativePtr);
                Cuda.ThrowCudaException(ret);

                switch (ret)
                {
                    case NativeMethods.ErrorType.MatrixElementTypeNotSupport:
                        throw new NotSupportedException($"{dataElementTypes} does not support");
                }
            }
        }

        /// <summary>
        /// Performs one stochastic gradient update step based on the mini-batch of data and labels supplied.
        /// </summary>
        /// <typeparam name="T">The type of element in the matrix.</typeparam>
        /// <param name="trainer">The trainer object of <see cref="LossMulticlassLogPerPixel"/>.</param>
        /// <param name="data">The training data.</param>
        /// <param name="label">The label.</param>
        /// <exception cref="ArgumentNullException"><paramref name="trainer"/>, <paramref name="data"/> or <paramref name="label"/> is null.</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="trainer"/> is disposed.</exception>
        /// <exception cref="NotSupportedException">The specified type of element in the matrix does not supported.</exception>
        public static void TrainOneStep<T>(DnnTrainer<LossMulticlassLogPerPixel> trainer, IEnumerable<Matrix<T>> data, IEnumerable<Matrix<ushort>> label)
            where T : struct
        {
            if (trainer == null)
                throw new ArgumentNullException(nameof(trainer));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (label == null)
                throw new ArgumentNullException(nameof(label));
            if (data.Count() != label.Count())
                throw new ArgumentException($"The count of {nameof(data)} must equal to {nameof(label)}'s.");

            trainer.ThrowIfDisposed();

            Matrix<T>.TryParse<T>(out var dataElementTypes);

            using (var dataVec = new StdVector<Matrix<T>>(data))
            using (var labelVec = new StdVector<Matrix<ushort>>(label))
            {
                var ret = NativeMethods.LossMulticlassLogPerPixel_trainer_train_one_step(trainer.Type,
                                                                                         trainer.NativePtr,
                                                                                         trainer.SolverType,
                                                                                         dataElementTypes.ToNativeMatrixElementType(),
                                                                                         dataVec.NativePtr,
                                                                                         NativeMethods.MatrixElementType.UInt16,
                                                                                         labelVec.NativePtr);
                Cuda.ThrowCudaException(ret);

                switch (ret)
                {
                    case NativeMethods.ErrorType.MatrixElementTypeNotSupport:
                        throw new NotSupportedException($"{dataElementTypes} does not support");
                }
            }
        }
        
        public override bool TryGetInputLayer<T>(T layer)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Overrides 

        /// <summary>
        /// Releases all unmanaged resources.
        /// </summary>
        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            if (this.NativePtr == IntPtr.Zero)
                return;

            NativeMethods.LossMulticlassLogPerPixel_delete(this.NetworkType, this.NativePtr);
        }

        public override string ToString()
        {
            var ofstream = IntPtr.Zero;
            var stdstr = IntPtr.Zero;
            var str = "";

            try
            {
                ofstream = NativeMethods.ostringstream_new();
                var ret = NativeMethods.LossMulticlassLogPerPixel_operator_left_shift(this.NetworkType, this.NativePtr, ofstream);
                switch (ret)
                {
                    case NativeMethods.ErrorType.OK:
                        stdstr = NativeMethods.ostringstream_str(ofstream);
                        str = StringHelper.FromStdString(stdstr);
                        break;
                    case NativeMethods.ErrorType.DnnNotSupportNetworkType:
                        throw new NotSupportNetworkTypeException(this.NetworkType);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (stdstr != IntPtr.Zero)
                    NativeMethods.string_delete(stdstr);
                if (ofstream != IntPtr.Zero)
                    NativeMethods.ostringstream_delete(ofstream);
            }

            return str;
        }

        #endregion

        public sealed class Subnet : DlibObject
        {

            #region Fields

            private readonly LossMulticlassLogPerPixel _Parent;

            #endregion

            #region Constructors

            internal Subnet(LossMulticlassLogPerPixel parent)
                : base(false)
            {
                if (parent == null)
                    throw new ArgumentNullException(nameof(parent));

                parent.ThrowIfDisposed();

                this._Parent = parent;

                var err = NativeMethods.LossMulticlassLogPerPixel_subnet(parent.NetworkType, parent.NativePtr, out var ret);
                this.NativePtr = ret;
            }

            #endregion

            #region Properties

            public Tensor Output
            {
                get
                {
                    this._Parent.ThrowIfDisposed();
                    var tensor = NativeMethods.LossMulticlassLogPerPixel_subnet_get_output(this._Parent.NetworkType, this.NativePtr, out var ret);
                    return new Tensor(tensor);
                }
            }

            #endregion

            #region Methods

            public LayerDetails GetLayerDetails()
            {
                this._Parent.ThrowIfDisposed();
                var ret = NativeMethods.LossMulticlassLogPerPixel_subnet_get_layer_details(this._Parent.NetworkType, this.NativePtr, out var value);
                return new LayerDetails(this._Parent, value);
            }

            #region Overrids

            protected override void DisposeUnmanaged()
            {
                base.DisposeUnmanaged();

                if (this.NativePtr == IntPtr.Zero)
                    return;

                NativeMethods.LossMulticlassLogPerPixel_subnet_delete(this._Parent.NetworkType, this.NativePtr);
            }

            #endregion

            #endregion

        }

        public sealed class LayerDetails : DlibObject
        {

            #region Fields

            private readonly LossMulticlassLogPerPixel _Parent;

            #endregion

            #region Constructors

            internal LayerDetails(LossMulticlassLogPerPixel parent, IntPtr ptr)
                : base(false)
            {
                if (parent == null)
                    throw new ArgumentNullException(nameof(parent));

                parent.ThrowIfDisposed();

                this._Parent = parent;
                this.NativePtr = ptr;
            }

            #endregion

            #region Methods

            public void SetNumFilters(int num)
            {
                this._Parent.ThrowIfDisposed();
                var ret = NativeMethods.LossMulticlassLogPerPixel_layer_details_set_num_filters(this._Parent.NetworkType, this.NativePtr, num);
                switch (ret)
                {
                    case NativeMethods.ErrorType.GeneralNotSupport:
                        throw new NotSupportedException();
                    case NativeMethods.ErrorType.DnnNotSupportNetworkType:
                        throw new NotSupportNetworkTypeException(this._Parent.NetworkType);
                }
            }

            #region Overrids

            protected override void DisposeUnmanaged()
            {
                base.DisposeUnmanaged();

                if (this.NativePtr == IntPtr.Zero)
                    return;

                //NativeMethods.loss_metric_subnet_delete(this._Parent.NetworkType, this.NativePtr);
            }

            #endregion

            #endregion

        }

        public sealed class LossDetails : DlibObject
        {

            #region Fields

            private readonly LossMulticlassLogPerPixel _Parent;

            #endregion

            #region Constructors

            internal LossDetails(LossMulticlassLogPerPixel parent, IntPtr ptr)
                : base(false)
            {
                if (parent == null)
                    throw new ArgumentNullException(nameof(parent));

                parent.ThrowIfDisposed();

                this._Parent = parent;
                this.NativePtr = ptr;
            }

            #endregion

        }

        private sealed class Output : OutputLabels<Matrix<ushort>>
        {

            #region Fields

            private readonly int _Size;

            #endregion

            #region Constructors

            internal Output(IntPtr output) :
                base(output)
            {
                this._Size = NativeMethods.dnn_output_stdvector_uint16_getSize(output);
            }

            #endregion

            #region Properties

            public override int Count
            {
                get
                {
                    this.ThrowIfDisposed();
                    return this._Size;
                }
            }

            public override Matrix<ushort> this[int index]
            {
                get
                {
                    this.ThrowIfDisposed();

                    if (!(0 <= index && index < this._Size))
                        throw new ArgumentOutOfRangeException();

                    var ptr = NativeMethods.dnn_output_stdvector_uint16_getItem(this.NativePtr, index);
                    return new Matrix<ushort>(ptr, 0, 0, false);
                }
            }

            public override Matrix<ushort> this[uint index]
            {
                get
                {
                    this.ThrowIfDisposed();

                    if (!(index < this._Size))
                        throw new ArgumentOutOfRangeException();

                    var ptr = NativeMethods.dnn_output_stdvector_uint16_getItem(this.NativePtr, (int)index);
                    return new Matrix<ushort>(ptr, 0, 0, false);
                }
            }

            #endregion

            #region Methods

            #region Overrides

            protected override void DisposeUnmanaged()
            {
                base.DisposeUnmanaged();

                if (this.NativePtr == IntPtr.Zero)
                    return;

                NativeMethods.dnn_output_stdvector_uint16_delete(this.NativePtr);
            }

            #endregion

            #endregion

            #region IEnumerable<TItem> Members

            public override IEnumerator<Matrix<ushort>> GetEnumerator()
            {
                this.ThrowIfDisposed();

                for (var index = 0; index < this._Size; index++)
                {
                    var ptr = NativeMethods.dnn_output_stdvector_uint16_getItem(this.NativePtr, index);
                    yield return new Matrix<ushort>(ptr, 0, 0, false);
                }
            }

            #endregion

        }

    }

}
