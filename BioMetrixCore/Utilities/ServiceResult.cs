using System.Collections.Generic;
using System.Net;

namespace BioMetrixCore.Utilities
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; } = true;
        public int Code { get; set; } = (int)HttpStatusCode.OK;
        public int SubCode { get; set; }
        public string UserMessage { get; set; }
        public string SystemMessage { get; set; }
        public T Data { get; set; }
        public ErrorData ErrorData { get; set; }
        public Dictionary<string, Cookie> Cookies { get; set; }
        public List<ValidateResult> _validateInfo;
        public List<ValidateResult> ValidateInfo
        {
            get
            {
                if (this._validateInfo == null)
                {
                    this._validateInfo = new List<ValidateResult>();
                }
                return this._validateInfo;
            }
            set
            {
                this._validateInfo = value;
            }
        }
        public int status { get; set; }
        public string error_message { get; set; }
    }

    public class ErrorData
    {
        public ErrorStatus Status { get; set; }
        public object Data { get; set; }
    }

    public class ErrorStatus
    {
        public string Type { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public bool Error { get; set; }
        public int ErrorCode { get; set; }
    }
    public class PagingResponse
    {
        public object SummaryData;

        public object PageData;

        public int Total { get; set; }

        //public bool IsCompressedData { get; set; }

        public object CustomData { get; set; }
        public PagingResponse() { }
        public PagingResponse(object data, int total, bool isCompressedData = false)
        {
            //this.IsCompressedData = isCompressedData;
            //if (isCompressedData && data != null)
            //{
            //this.PageData = Converter.Compress(Converter.Serialize(data));
            //}
            //else
            //{
            this.PageData = data;
            //}
            this.Total = total;
        }
    }
    public class ValidateResult
    {
        /// <summary>
        /// ID của bản ghi lỗi
        /// </summary>
        public object ID { get; set; }
        /// <summary>
        /// Mã lỗi
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Nội dung lỗi
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Dữ liệu tùy biến mang thêm
        /// </summary>
        public object AdditionInfo { get; set; }

        /// <summary>
        /// Kiểu validate
        /// </summary>
        public string ValidateType { get; set; }
    }
}
