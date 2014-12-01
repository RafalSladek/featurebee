$(function () {
    $.widget("as24.qr", {
        _create: function () {
            var self = this;
            $(self.element).find('[data-description="QR-Code Generation"]').find("p").hide();

            $(self.element).find('[data-generate="qr-code"]').click(function () {
                $(self.element).find('[data-description="QR-Code Generation"]').find("p").show();
                var url = $(self.element).find('[name="urlQRCode"]').val();
                var featureName = $(self.element).find('[name="name"]').val();
                var featureEnabled = url + "?" + $.param({ "FeatureBee": '#' + featureName + "=true#" });
                var featureDisabled = url + "?" + $.param({ "FeatureBee": '#' + featureName + "=false#" });

                $(self.element).find('#qrcode_enable').empty();
                $(self.element).find('#qrcode_enable').qrcode(featureEnabled);
                $(self.element).find('#qrcode_disable').empty();
                $(self.element).find('#qrcode_disable').qrcode(featureDisabled);
            });
        }
    });
})