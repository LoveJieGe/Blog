﻿if (!$.blockUI) { throw new Error("HxLoad requires blockUI"); }
; (function ($, window) {
    "use strict";
    var hxLoad = {
    };
    var blockUI = function (opt) {
        opt = $.extend(true, {}, opt);
        var me = this,
            loadCls = 'hx-loading-fading-circle',
            html = '',
            color = (opt.color || '').toLowerCase(),
            label = '加载中...',
            labelAlign = (opt.labelAlign || '').toLowerCase(),
            hxStyle = '';
        //信息
        if (opt.label === false) {
            label = '';
        } else if (opt.label) {
            label = opt.label === true ? label : opt.label;
        }
        //信息方向
        if (labelAlign === 'left' || labelAlign === 'l') {
            hxStyle += 'flex-direction: row-reverse;-webkit-flex-direction: row-reverse;';
        } else if (labelAlign === 'top' || labelAlign === 't') {
            hxStyle += 'flex-direction: column-reverse;-webkit-flex-direction: column-reverse;';
        } else if (labelAlign === 'bottom' || labelAlign === 'b') {
            hxStyle += 'flex-direction: column;-webkit-flex-direction: column;';
        } else {
            hxStyle += 'flex-direction: row;-webkit-flex-direction: row;';
        }
        //信息模式
        var mode = (opt.mode || '').toLowerCase();
        if (mode === 'bounce' || mode === 'b') {
            loadCls = 'hx-loading-bounce';
            html = `<div class='hx-loading' style="${hxStyle}">
                        <div class="${loadCls} hx-loading-${color}">
                            <div class="bounce-child bounce1"></div>
                            <div class="bounce-child bounce2"></div>
                            <div class="bounce-child bounce3"></div>
                        </div>
                        <span>&nbsp;&nbsp;${label}
                    </div>
                `;
        } else if (mode === 'circle' || mode === 'c') {
            loadCls = 'hx-loading-circle';
            html += `<div class='hx-loading' style="${hxStyle}">`;
            html += `<div class="${loadCls} hx-loading-${color}">`;
            for (var i = 1; i <= 12; i++) {
                html += `<div class="circle${i} circle-child"></div>`;
            }
            html += '</div>';
            html += `<span style='display:block;'>&nbsp;&nbsp;${label}`;
            html += '</div>';
        } else {
            html += `<div class='hx-loading' style="${hxStyle}">`;
            html += `<div class="${loadCls} hx-loading-${color}">`;
            for (var j = 1; j <= 12; j++) {
                html += `<div class="fadecircle${j} circle-child"></div>`;
            }
            html += '</div>';
            html += `<span>&nbsp;&nbsp;${label}`;
            html += '</div>';
        }
        var blockOpt = {
            message: html,
            baseZ: opt.zIndex ? opt.zIndex : 1000,
            centerY: opt.cenrerY !== undefined ? opt.cenrerY : true,
            centerX: opt.centerX !== undefined ? opt.centerX : true, 
            // (hat tip to Jorge H. N. de Vasconcelos) 
            iframeSrc: /^https/i.test(window.location.href || '') ? 'javascript:false' : 'about:blank',
            // force usage of iframe in non-IE browsers (handy for blocking applets) 
            forceIframe: false, 
            // fadeIn time in millis; set to 0 to disable fadeIn on block 
            fadeIn: opt.fadeIn ? opt.fadeIn:200,
            // fadeOut time in millis; set to 0 to disable fadeOut on unblock 
            fadeOut: opt.fadeOut ? opt.fadeOut :400,
            // time in millis to wait before auto-unblocking; set to 0 to disable auto-unblock 
            timeout: opt.timeout ? opt.timeout:0,
            css: {
                border: '0',
                padding: '0',
                backgroundColor: 'none'
            },
            overlayCSS: {
                backgroundColor: opt.overlayColor ? opt.overlayColor : '#555',
                opacity: opt.opacity ? opt.opacity: 0.1,
                cursor: 'wait'
            }
        };
        if (opt.target) {
            var el = $(opt.target);
            if (el.height() <= $(window).height()) {
                blockOpt.cenrerY = true;
            }
            el.block(blockOpt);
        } else { // page blocking
            $.blockUI(blockOpt);
        }
    },
        unblockUI = function(el) {
            if (el) {
                $(el).unblock({
                    onUnblock: function () {
                        $(el).css('position', '');
                        $(el).css('zoom', '');
                    }
                });
            } else {
                $.unblockUI();
            }
        };
    
    window.HxLoad = $.extend(true, hxLoad, {
        blockUI(option) {
            blockUI(option);
        },
        unblockUI(el) {
            unblockUI(el);
        }
    });
})(jQuery, window);