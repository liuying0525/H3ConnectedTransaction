// SheetHyperLink控件

; (function ($) {

    //控件执行
    $.fn.SheetHyperLink = function () {
        return $.MvcSheetUI.Run.call(this, "SheetHyperLink", arguments);
    };

    $.MvcSheetUI.Controls.SheetHyperLink = function (element, options, sheetInfo) {
        $.MvcSheetUI.Controls.SheetHyperLink.Base.constructor.call(this, element, options, sheetInfo);
    };

    $.MvcSheetUI.Controls.SheetHyperLink.Inherit($.MvcSheetUI.IControl, {
        Render: function () {
            var $element = $(this.Element);

            //是否可见
            if (!this.Visiable) {
                $element.hide();
                return;
            }

            //text
            var text = this.Text;
            if (!text) {
                text = this.NavigateUrl;
            }
            if (this.TextDataField) {
                text = $.MvcSheetUI.GetControlValue(this.TextDataField, this.RowNum);
            }
            $element.text(text);
            $element.addClass("SheetHyperLink").addClass("printHidden");
            $("<span></span>").addClass("viewHidden").text(text).insertAfter($element);

            //href
            var href = this.NavigateUrl;
            if (this.NavigateUrlDataField) {
                href = $.MvcSheetUI.GetControlValue(this.NavigateUrlDataField, this.RowNum);
            }
            $element.attr("target", "_blank");
            $element.attr("href", href);
        },
        RenderMobile: function () {
            this.Render();
        },
        //链接不允许输入,永不校验
        Validate: function () {
            return true;
        },
        //返回数据值
        SaveDataField: function () {
            result = {};
            result[this.DataField] = this.DataItem;
            var href = this.NavigateUrl;
            if (this.NavigateUrlDataField) {
                href = $.MvcSheetUI.GetControlValue(this.NavigateUrlDataField, this.RowNum);
            }
            result[this.DataField].V = href;
            return result;
        },
        //获取数据值
        GetValue: function () {
            return this.NavigateUrl;
        }
    });
})(jQuery);