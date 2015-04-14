﻿using System.Linq;
using Motillo.Nop.Plugin.KlarnaCheckout.Controllers;
using Motillo.Nop.Plugin.KlarnaCheckout.Data;
using Motillo.Nop.Plugin.KlarnaCheckout.Services;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Localization;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Routing;

namespace Motillo.Nop.Plugin.KlarnaCheckout
{
    public class KlarnaCheckoutProcessor : BasePlugin, IPaymentMethod
    {
        private readonly IKlarnaCheckoutHelper _klarnaCheckout;
        private readonly IWorkContext _workContext;

        public KlarnaCheckoutProcessor(
            IKlarnaCheckoutHelper klarnaCheckout,
            IWorkContext workContext)
        {
            _klarnaCheckout = klarnaCheckout;
            _workContext = workContext;
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            var match = _klarnaCheckout.GetSupportedLocale();

            return match == null;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0m;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException("order");
            }

            return false;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "KlarnaCheckout";
            routeValues = new RouteValueDictionary
            {
                { "Namespaces", "Motillo.Nop.Plugin.KlarnaCheckout.Controllers" }, { "area", null }
            };
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "KlarnaCheckout";
            routeValues = new RouteValueDictionary
            {
                { "Namespaces", "Motillo.Nop.Plugin.KlarnaCheckout.Controllers" }, { "area", null }
            };
        }

        public Type GetControllerType()
        {
            return typeof(KlarnaCheckoutController);
        }

        public bool SupportCapture { get { return false; } }
        public bool SupportPartiallyRefund { get { return false; } }
        public bool SupportRefund { get { return false; } }
        public bool SupportVoid { get { return false; } }
        public RecurringPaymentType RecurringPaymentType { get { return RecurringPaymentType.NotSupported; } }
        public PaymentMethodType PaymentMethodType { get {return PaymentMethodType.Standard;} }
        public bool SkipPaymentInfo { get { return false; } }

        public override void Install()
        {
            var context = EngineContext.Current.Resolve<KlarnaCheckoutContext > ();
            context.Install();

            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.EId", "Butiks-ID (EID)");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.SharedSecret", "Shared secret");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.EnabledCountries", "Enabled countries");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.EnabledCountries.Hint", "Comma separated list with the supported countries two letter ISO code. E.g. SE,NO");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.TermsUrl", "Terms URL");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.CheckoutUrl", "Checkout URL");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.DisableAutofocus", "Disable autofocus");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.TestMode", "Test mode");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorButton", "Button color");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorButtonText", "Button text color");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorCheckbox", "Checkbox color");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorCheckboxCheckmark", "Checkbox checkmark color");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorHeader", "Header color");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorLink", "Link color");

            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Text.RenderingError", "Error showing Klarna Checkout");
            this.AddOrUpdatePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Text.Unauthorized", "Unauthorized.");
            
            base.Install();
        }

        public override void Uninstall()
        {
            var context = EngineContext.Current.Resolve<KlarnaCheckoutContext>();
            context.Uninstall();

            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.EId");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.SharedSecret");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.EnabledCountries");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.EnabledCountries.Hint");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.TermsUrl");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.CheckoutUrl");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.DisableAutofocus");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.TestMode");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorButton");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorButtonText");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorCheckbox");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorCheckboxCheckmark");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorHeader");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Settings.ColorLink");

            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Text.RenderingError");
            this.DeletePluginLocaleResource("Motillo.Plugin.KlarnaCheckout.Text.Unauthorized");

            base.Uninstall();
        }
    }
}