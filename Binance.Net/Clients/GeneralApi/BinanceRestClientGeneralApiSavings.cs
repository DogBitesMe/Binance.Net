﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.Converters;
using Binance.Net.Enums;
using Binance.Net.Interfaces.Clients.GeneralApi;
using Binance.Net.Objects.Models.Spot.Lending;
using Binance.Net.Objects.Models.Spot.Savings;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;

namespace Binance.Net.Clients.GeneralApi
{
    /// <inheritdoc />
    public class BinanceRestClientGeneralApiSavings : IBinanceRestClientGeneralApiSavings
    {
        // Savings(Simple-Earn)
        private const string flexibleProductListEndpoint = "simple-earn/flexible/list";
        private const string subscribeFlexibleProductListEndpoint = "simple-earn/flexible/subscribe";
        private const string redeemFlexibleProductEndpoint = "simple-earn/flexible/redeem";
        private const string getFlexiblePersonalLeftQuotaEndpoint = "simple-earn/flexible/personalLeftQuota";
        private const string getFlexibleProductPositionEndpoint = "simple-earn/flexible/position";
        private const string getFlexibleSubscriptionRecordEndpoint = "simple-earn/flexible/history/subscriptionRecord";
        private const string getFlexibleRedemptionRecordEndpoint = "simple-earn/flexible/history/redemptionRecord";

        private const string leftDailyRedemptionQuotaEndpoint = "lending/daily/userRedemptionQuota";
        
        
        private const string fixedAndCustomizedFixedProjectListEndpoint = "lending/project/list";
        private const string purchaseCustomizedFixedProjectEndpoint = "lending/customizedFixed/purchase";
        private const string fixedAndCustomizedProjectPositionEndpoint = "lending/project/position/list";
        private const string lendingAccountEndpoint = "lending/union/account";
        private const string purchaseRecordEndpoint = "lending/union/purchaseRecord";
        private const string redemptionRecordEndpoint = "lending/union/redemptionRecord";
        private const string lendingInterestHistoryEndpoint = "lending/union/interestHistory";
        private const string positionChangedEndpoint = "lending/positionChanged";

        private readonly BinanceRestClientGeneralApi _baseClient;

        internal BinanceRestClientGeneralApiSavings(BinanceRestClientGeneralApi baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Flexible Product List
        /// <inheritdoc />
        public async Task<WebCallResult<BinanceFlexibleProductList>> GetFlexibleProductListAsync(string asset = null, int? page = null, int? pageSize = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("size", pageSize?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
            //A.L($"[GetFlexibleProductListAsync]Before SendRequestInternal");
            var ret = await _baseClient.SendRequestInternal<BinanceFlexibleProductList>(_baseClient.GetUrl(flexibleProductListEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            //A.L($"[GetFlexibleProductListAsync]After SendRequestInternal:{ret.Success} rows:{ret.Data.Rows.Count}");
            return ret;
        }

        #endregion

        #region Subscribe Flexible Product

        /// <inheritdoc />
        public async Task<WebCallResult<BinanceSimpleEarnPurchaseResult>> SubscribeFlexibleProductAsync(string productId,
            decimal quantity, bool? autoSubscribe, SourceAccount? sourceAccount, long? receiveWindow = null, CancellationToken ct = default)
        {
            productId.ValidateNotNull(nameof(productId));

            var parameters = new Dictionary<string, object>
            {
                { "productId", productId },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("autoSubscribe", autoSubscribe?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("sourceAccount", sourceAccount?.ToString(CultureInfo.InvariantCulture));

            parameters.AddOptionalParameter("recvWindow",
                receiveWindow?.ToString(CultureInfo.InvariantCulture) ??
                _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient
                .SendRequestInternal<BinanceSimpleEarnPurchaseResult>(
                    _baseClient.GetUrl(subscribeFlexibleProductListEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters,
                    true).ConfigureAwait(false);
        }

        #endregion

        #region Get Flexible Personal Left Quota
        /// <inheritdoc />
        public async Task<WebCallResult<BinanceFlexiblePersonalQuotaLeft>> GetFlexiblePersonalLeftQuotaAsync(string productId, long? receiveWindow = null, CancellationToken ct = default)
        {
            productId.ValidateNotNull(nameof(productId));

            var parameters = new Dictionary<string, object>
            {
                { "productId", productId }
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceFlexiblePersonalQuotaLeft>(_baseClient.GetUrl(
                getFlexiblePersonalLeftQuotaEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion


        #region Get Left Daily Redemption Quota of Flexible Product
        /// <inheritdoc />
        public async Task<WebCallResult<BinanceRedemptionQuotaLeft>> GetLeftDailyRedemptionQuotaOfFlexibleProductAsync(string productId, RedeemType type, long? receiveWindow = null, CancellationToken ct = default)
        {
            productId.ValidateNotNull(nameof(productId));

            var parameters = new Dictionary<string, object>
            {
                { "productId", productId },
                { "type",  JsonConvert.SerializeObject(type, new RedeemTypeConverter(false)) }
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceRedemptionQuotaLeft>(_baseClient.GetUrl(leftDailyRedemptionQuotaEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region Redeem Flexible Product
        /// <inheritdoc />
        public async Task<WebCallResult<RedeemProductResult>> RedeemFlexibleProductAsync(string productId, decimal quantity,
            bool? redeemAll, SourceAccount?
                destAccount, long? receiveWindow = null, CancellationToken ct = default)
        {
            productId.ValidateNotNull(nameof(productId));

            var parameters = new Dictionary<string, object>
            {
                { "productId", productId },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("redeemAll", redeemAll?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("destAccount", destAccount?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<RedeemProductResult>(_baseClient.GetUrl(redeemFlexibleProductEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region Get Flexible Product Position
        /// <inheritdoc />
        public async Task<WebCallResult<BinanceFlexibleProductPositionList>> GetFlexibleProductPositionAsync(string? asset = null, string? productId = null, int? page = null, int? pageSize = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("productId", productId);
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("size", pageSize?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceFlexibleProductPositionList>(_baseClient.GetUrl(
                getFlexibleProductPositionEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        /// <inheritdoc />
        public async Task<WebCallResult<SubscriptionRecordList>> GetFlexibleSubscriptionRecordAsync(
            string? productId, string? purchaseId, string? asset, DateTime? startTime, DateTime? endTime, int? current,
            int? size, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("productId", productId);
            parameters.AddOptionalParameter("purchaseId", purchaseId);
            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("current", current?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("size", size?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<SubscriptionRecordList>(_baseClient.GetUrl(
                getFlexibleSubscriptionRecordEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<RedemptionRecordList>> GetFlexibleRedemptionRecordAsync(string? productId,
            string? redeemId, string? asset, DateTime? startTime, DateTime? endTime, int? current, int? size,
            long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("productId", productId);
            parameters.AddOptionalParameter("redeemId", redeemId);
            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("current", current?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("size", size?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow",
                receiveWindow?.ToString(CultureInfo.InvariantCulture) ??
                _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<RedemptionRecordList>(_baseClient.GetUrl(
                    getFlexibleRedemptionRecordEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true)
                .ConfigureAwait(false);
        }

        #region Get Fixed And Customized Fixed Project List
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BinanceProject>>> GetFixedAndCustomizedFixedProjectListAsync(
            ProjectType type, string? asset = null, ProductStatus? status = null, bool? sortAscending = null, string? sortBy = null, int? currentPage = null, int? size = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", JsonConvert.SerializeObject(type, new ProjectTypeConverter(false)) }
            };
            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("status", status == null ? null : JsonConvert.SerializeObject(status, new ProductStatusConverter(false)));
            parameters.AddOptionalParameter("isSortAsc", sortAscending.ToString().ToLower());
            parameters.AddOptionalParameter("sortBy", sortBy);
            parameters.AddOptionalParameter("current", currentPage);
            parameters.AddOptionalParameter("size", size);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceProject>>(_baseClient.GetUrl(fixedAndCustomizedFixedProjectListEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion

        #region Purchase Customized Fixed Project
        /// <inheritdoc />
        public async Task<WebCallResult<BinanceLendingPurchaseResult>> PurchaseCustomizedFixedProjectAsync(string projectId, int lot, long? receiveWindow = null, CancellationToken ct = default)
        {
            projectId.ValidateNotNull(nameof(projectId));

            var parameters = new Dictionary<string, object>
            {
                { "projectId", projectId },
                { "lot", lot.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceLendingPurchaseResult>(_baseClient.GetUrl(purchaseCustomizedFixedProjectEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion

        #region Get Customized Fixed Project Position
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BinanceCustomizedFixedProjectPosition>>> GetCustomizedFixedProjectPositionsAsync(string asset, string? projectId = null, ProjectStatus? status = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));

            var parameters = new Dictionary<string, object>
            {
                { "asset", asset }
            };
            parameters.AddOptionalParameter("projectId", projectId);
            parameters.AddOptionalParameter("status", status == null ? null : JsonConvert.SerializeObject(status, new ProjectStatusConverter(false)));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceCustomizedFixedProjectPosition>>(_baseClient.GetUrl(fixedAndCustomizedProjectPositionEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region Lending Account
        /// <inheritdoc />
        public async Task<WebCallResult<BinanceLendingAccount>> GetLendingAccountAsync(long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceLendingAccount>(_baseClient.GetUrl(lendingAccountEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region Get Purchase Records
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BinancePurchaseRecord>>> GetPurchaseRecordsAsync(LendingType lendingType, string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? page = 1, int? limit = 10, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "lendingType", JsonConvert.SerializeObject(lendingType, new LendingTypeConverter(false)) }
            };
            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("size", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinancePurchaseRecord>>(_baseClient.GetUrl(purchaseRecordEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region Get Redemption Record
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BinanceRedemptionRecord>>> GetRedemptionRecordsAsync(LendingType lendingType, string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? page = 1, int? limit = 10, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "lendingType", JsonConvert.SerializeObject(lendingType, new LendingTypeConverter(false)) }
            };
            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("size", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceRedemptionRecord>>(_baseClient.GetUrl(redemptionRecordEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region Get Interest History
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BinanceLendingInterestHistory>>> GetLendingInterestHistoryAsync(LendingType lendingType, string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? page = 1, int? limit = 10, long? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "lendingType", JsonConvert.SerializeObject(lendingType, new LendingTypeConverter(false)) }
            };
            parameters.AddOptionalParameter("asset", asset);
            parameters.AddOptionalParameter("size", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("current", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToMilliseconds(endTime));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<IEnumerable<BinanceLendingInterestHistory>>(_baseClient.GetUrl(lendingInterestHistoryEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region ChangeToDailyPosition
        /// <inheritdoc />
        public async Task<WebCallResult<BinanceLendingChangeToDailyResult>> ChangeToDailyPositionAsync(string projectId, int lot, long? positionId = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            projectId.ValidateNotNull(nameof(projectId));

            var parameters = new Dictionary<string, object>
            {
                { "projectId", projectId },
                { "lot", lot.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("positionId", positionId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.ClientOptions.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<BinanceLendingChangeToDailyResult>(_baseClient.GetUrl(positionChangedEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion
    }
}
