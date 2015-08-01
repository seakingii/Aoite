(function () {

    if (!Date.prototype.format)
        Date.prototype.format = function (fmt) {
            if (!fmt) fmt = 'yyyy-MM-dd HH:mm:ss';
            var o = {
                "M+": this.getMonth() + 1, //月份         
                "d+": this.getDate(), //日         
                "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时         
                "H+": this.getHours(), //小时         
                "m+": this.getMinutes(), //分         
                "s+": this.getSeconds(), //秒         
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度         
                "S": this.getMilliseconds() //毫秒         
            };
            var week = {
                "0": "/u65e5",
                "1": "/u4e00",
                "2": "/u4e8c",
                "3": "/u4e09",
                "4": "/u56db",
                "5": "/u4e94",
                "6": "/u516d"
            };
            if (/(y+)/.test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            if (/(E+)/.test(fmt)) {
                fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468") : "") + week[this.getDay() + ""]);
            }
            for (var k in o) {
                if (new RegExp("(" + k + ")").test(fmt)) {
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
                }
            }
            return fmt;
        };
    if (!Array.prototype.remove)
        Array.prototype.remove = function (item) {
            var index = this.indexOf(item);
            if (index >= 0) this.splice(index, 1);
        };
    if (!jQuery.prototype.serializeObject)
        jQuery.prototype.serializeObject = function () {
            var obj = new Object();
            $.each(this.serializeArray(), function (index, param) {
                if (!(param.name in obj)) {
                    obj[param.name] = param.value;
                }
            });
            return obj;
        };

    if (!window.async) window.async = function (callback, e) {
        if (e === false) callback();
        else setTimeout(callback, e | 0);
    }

    if (!window.reloadNow) window.reloadNow = function () {
        /// <summary>
        ///     立即刷新当前页面。
        /// </summary>
        document.location.reload(true);
    };
    if (!window.test) window.test = function (r, url) {
        /// <summary>
        ///     测试指定的结果。
        /// </summary>
        /// <param name="r" type="Object">
        ///     一个结果。
        /// </param>
        /// <param name="url" type="String">
        ///     若有值则表示成功后跳转的 URL 地址。
        /// </param>
        /// <returns type="Boolean" />

        if (r.status) {
            if (window.msg) window.msg.error(r.message);
            else alert(r.message);
            return false;
        }
        if (url) document.location.href = url;
        return true;
    }
    if (!window.testReload) window.testReload = function (r, successCallback) {
        /// <summary>
        ///     测试指定的结果。若结果成功，则刷新当前页面。
        /// </summary>
        /// <param name="r" type="Object">
        ///     一个结果。
        /// </param>
        /// <param name="successCallback" type="function">
        ///     成功时候的回调函数。
        /// </param>
        /// <returns type="Boolean" />

        if (test(r)) {
            if (successCallback) successCallback(r);
            reloadNow();
            return true;
        }
        return false;
    }

    if (!console) console = { log: function (m) { } };

    var aoite = window.aoite || (window.aoite = {});
    aoite.$settings = {
        baseUrl: "/",
        getDefaultResult: function () { return { status: -1, message: "系统发生了未知异常。请不要关闭此页面，并联系系统管理员。", value: undefined } },
        error: function (status, message) { },
        success: function (data) { },
        remote: function (url, ps, options) {
            /// <summary>
            ///     同步 AJAX 远程调用。
            /// </summary>
            /// <param name="url" type="String">
            ///     Ajax 连接地址。
            /// </param>
            /// <param name="ps" type="Object">
            ///     Ajax 数据参数。
            /// </param>
            /// <param name="options" type="Object">
            ///     Ajax 扩展配置。
            /// </param>
            /// <returns type="Object" />
            if (ps.$model) ps = ps.$model;
            var settings = this.$settings || this;
            var result;

            options = $.extend(options || {}, {
                url: settings.baseUrl + url,
                data: JSON.stringify(ps),
                success: function (data, textStatus, jqXHR) {
                    if (settings.success) settings.success(data);
                    if (data.status && data.status == 401) top.document.location.href = data.value;
                    result = data;
                },
                contentType: "application/json; charset=UTF-8",
                dataType: "json",
                type: "POST",
                cache: false,
                async: false,
                error: function (jqXHR, status, error) {
                    if (settings.error && settings.error(jqXHR.status, jqXHR.responseText) === false) return;
                    if (jqXHR.status == 401) {
                        top.document.location.reload(true);
                        return;
                    }
                    if (console) {
                        console.log("错误：" + jqXHR.status);
                        console.log(jqXHR.responseText);
                    }
                    result = settings.getDefaultResult();
                }
            });
            $.ajax(options);
            return result;
        }
    }

    aoite.remote = aoite.$settings.remote;
})();