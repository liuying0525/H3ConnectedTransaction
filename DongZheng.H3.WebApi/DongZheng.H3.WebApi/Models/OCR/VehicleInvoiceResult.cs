using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.OCR
{
    public class DZVehicleInvoiceResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public VehicleInvoiceResult data { get; set; }
    }
    public class VehicleInvoiceResult
    {
        public string type { get; set; }

        [JsonProperty("vehicle_invoice_buyer")]
        public string vehicleInvoiceBuyer { get; set; }

        [JsonProperty("vehicle_invoice_buyer_id")]
        public string vehicleInvoiceBuyerId { get; set; }

        [JsonProperty("vehicle_invoice_car_model")]
        public string vehicleInvoiceCarModel { get; set; }

        [JsonProperty("vehicle_invoice_car_made_place")]
        public string vehicleInvoiceCarMadePlace { get; set; }

        [JsonProperty("vehicle_invoice_cert_id")]
        public string vehicleInvoiceCertId { get; set; }

        [JsonProperty("vehicle_invoice_engine_id")]
        public string vehicleInvoiceEngineId { get; set; }

        [JsonProperty("vehicle_invoice_car_vin")]
        public string vehicleInvoiceCarVin { get; set; }

        [JsonProperty("vehicle_invoice_total_price")]
        public string vehicleInvoiceTotalPrice { get; set; }

        [JsonProperty("vehicle_invoice_total_price_digits")]
        public string vehicleInvoiceTotalPriceDigits { get; set; }

        [JsonProperty("vehicle_invoice_telephone")]
        public string vehicleInvoiceTelephone { get; set; }

        [JsonProperty("vehicle_invoice_issue_date")]
        public string vehicleInvoiceIssueDate { get; set; }

        [JsonProperty("vehicle_invoice_daima")]
        public string vehicleInvoiceDaima { get; set; }

        [JsonProperty("vehicle_invoice_haoma")]
        public string vehicleInvoiceHaoma { get; set; }

        [JsonProperty("vehicle_invoice_dealer")]
        public string vehicleInvoiceDealer { get; set; }

        [JsonProperty("vehicle_invoice_jida_daima")]
        public string vehicleInvoiceJidaDaima { get; set; }

        [JsonProperty("vehicle_invoice_jida_haoma")]
        public string vehicleInvoiceJidaHaoma { get; set; }

        [JsonProperty("vehicle_invoice_tax_rate")]
        public string vehicleInvoiceTaxRate { get; set; }

        [JsonProperty("vehicle_invoice_tax_amount")]
        public string vehicleInvoiceTaxAmount { get; set; }

        [JsonProperty("vehicle_invoice_price_without_tax")]
        public string vehicleInvoicePriceWithoutTax { get; set; }

        public string fileUrl { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>

        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }
    }
}