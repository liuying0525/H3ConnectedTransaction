(function (w) {
    var custName, certNo;

    //ocr智能识别
    function ocr_intelligent_identification() {
        custName = $("#sqr_name").find("span").html();
        certNo = $("#sqr_id").find("span").html();
        var instanceId = $.MvcSheetUI.SheetInfo.InstanceId;
        var accountNumber = $.MvcSheetUI.GetControlValue("AccoutNumber");
        var vin_number = $.MvcSheetUI.GetControlValue("vin_number");
        var application_number = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");

        bindIdCartHtml(instanceId);

        bindBankCardHtml(instanceId, accountNumber);

        bindVehicleInvoiceHtml(instanceId, vin_number);

        bindContractHtml(instanceId, application_number, vin_number);

        bindDeductionsAuthorizedHtml(instanceId, application_number, accountNumber);

        bindPolicyHtml(instanceId, vin_number);
    }

    w.ocr_intelligent_identification = ocr_intelligent_identification;

    /** 数字金额大写转换(可以处理整数,小数,负数) */
    function smalltoBIG(n, yuan) {
        var fraction = ['角', '分'];
        var digit = ['零', '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖'];
        var unit = [['' + yuan + '', '万', '亿'], ['', '拾', '佰', '仟']];
        var head = n < 0 ? '欠' : '';
        n = Math.abs(n);

        var s = '';

        for (var i = 0; i < fraction.length; i++) {
            s += (digit[Math.floor(n * 10 * Math.pow(10, i)) % 10] + fraction[i]).replace(/零./, '');
        }
        s = s || '整';
        n = Math.floor(n);

        for (var i = 0; i < unit[0].length && n > 0; i++) {
            var p = '';
            for (var j = 0; j < unit[1].length && n > 0; j++) {
                p = digit[n % 10] + unit[1][j] + p;
                n = Math.floor(n / 10);
            }
            s = p.replace(/(零.)*零$/, '').replace(/^$/, '零') + unit[0][i] + s;
        }
        return head + s.replace(/(零.)*零元/, '元').replace(/(零.)+/g, '零').replace(/^整$/, '零元整');
    }

    // 绑定身份证HTML
    function bindIdCartHtml(instanceId) {
        var url = '/Portal/OCR/GetIdCardResult?instanceID=' + instanceId;
        var idCardJson = getData(url);
        var idCardItem = '', isConsistent = false;

        jQuery.each(idCardJson, function (i, val) {
            if (val.name == custName && val.idNumber == certNo) {
                isConsistent = true;
            }

            if (val.name || val.idNumber) {
                idCardItem += '<p>' + val.name + '&' + val.idNumber + '</p>';
            }
        });

        var idCardResultInfoHtml = '<tr><td class="col-md-2">客户姓名&身份证号</td><td class="col-md-2"><span>' + (idCardItem || '无记录') + '</span></td><td class="col-md-2">' + custName + '&' + certNo + '</td>';
        var idCardContent = "";
        if (idCardJson.length == 0) {
            idCardContent += '<td colspan="3" class="col-md-2">身份证号&客户姓名：<span style="color: red">无法识别</span></td>';
            idCardResultInfoHtml += '<td class="col-md-2"><span style="color: red">不一致</span></td></tr>';
        } else {
            if (!isConsistent) {
                idCardContent += '<td colspan="3" class="col-md-2">身份证号或客户姓名：<span style="color: red">不一致</span></td>';
                idCardResultInfoHtml += '<td class="col-md-2"><span style="color: red">不一致</span></td></tr>';
            } else {
                idCardContent += '<td colspan="3" class="col-md-2"></td>';
                idCardResultInfoHtml += '<td class="col-md-2">一致</td></tr>';
            }
        }

        if (!isConsistent) {
            $('#ocr').append('<tr><td class="col-md-2">身份证</td>' + idCardContent + '</tr>');
        }

        $('#idCardResultInfo').append(idCardResultInfoHtml);
    }

    // 绑定银行卡HTML
    function bindBankCardHtml(instanceId, accountNumber) {
        var url = '/Portal/OCR/GetBankCardResult?instanceID=' + instanceId;
        var bankCardJson = getData(url);
        var reg = new RegExp(" ", "g");
        var bankCardResultInfoHtml = '<tr><td class="col-md-2">银行卡号</td>';
        var bankContent = '', isMatch = false;

        if ($.isEmptyObject(bankCardJson)) {
            bankContent += '<td colspan="3" class="col-md-2">银行卡号：<span style="color: red">无法识别</span></td>';
            bankCardResultInfoHtml += '<td class="col-md-2"><span style="color: red">无记录<span></td><td class="col-md-2">' + accountNumber + '</td><td class="col-md-2"><span style="color: red">不一致</span></td></tr>';
        } else {
            var cardNumberreg = bankCardJson.cardNumber.replace(reg, '');
            var compareArr = startCompare(cardNumberreg, accountNumber);
            if (cardNumberreg != accountNumber) {
                bankContent += '<td colspan="3" class="col-md-2">银行卡号：<span style="color: red">不一致</span></td>';
                bankCardResultInfoHtml += '<td class="col-md-2">' + compareArr[0].join('') + '</td><td class="col-md-2">' + compareArr[1].join('') + '</td><td class="col-md-2"><span style="color: red">不一致</span></td></tr>';
            } else {
                isMatch = true;
                bankContent += '<td colspan="3" class="col-md-2"></td>';
                bankCardResultInfoHtml += '<td class="col-md-2">' + cardNumberreg + '</td><td class="col-md-2">' + accountNumber + '</td><td class="col-md-2">一致</td></tr>';
            }
        }

        if (!isMatch) {
            $('#ocr').append('<tr><td class="col-md-2">银行卡</td>' + bankContent + '</tr>');
        }

        $('#idCardResultInfo').append(bankCardResultInfoHtml);
    }

    // 绑定购车发票HTML
    function bindVehicleInvoiceHtml(instanceId, vinNumber) {
        var getReceiveTimeUrl = '/Portal/WorkItemfinished/GetInvoiceTimeResult?instanceID=' + instanceId;
        //获取开票时间
        var receiveTime = getData(getReceiveTimeUrl).receiveTime;
        var getVehicleInvoiceUrl = '/Portal/OCR/GetVehicleInvoiceResult?instanceID=' + instanceId;
        var vehicleInvoiceJson = getData(getVehicleInvoiceUrl);
        var asset_sale_price = smalltoBIG($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SALE_PRICE", 1), "圆");
        var jxs = $.MvcSheetUI.GetElement("BUSINESS_PARTNER_ID").find("option:selected").text();
        var counter, length, vehicleInvoiceResult, vehicleInvoiceResultInfoHtml = "", arrayVehicleInvoiceObj = [], compareArr;

        if ($.isEmptyObject(vehicleInvoiceJson)) {
            vehicleInvoiceResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">购买方姓名</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ custName + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">证件号码</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ certNo + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">发票金额</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ asset_sale_price + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">车架号</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ vinNumber + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">销货单位</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ jxs + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">开票日期</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ receiveTime + "</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>";
            arrayVehicleInvoiceObj.push("购买方姓名");
            arrayVehicleInvoiceObj.push("证件号码");
            arrayVehicleInvoiceObj.push("发票金额");
            arrayVehicleInvoiceObj.push("车架号");
            arrayVehicleInvoiceObj.push("销货单位");
        } else {
            vehicleInvoiceResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">购买方姓名</td>\
                                            <td class=\"col-md-2\">"+ vehicleInvoiceJson.vehicleInvoiceBuyer + "</td>\
                                            <td class=\"col-md-2\">"+ custName + "</td>";
            if (vehicleInvoiceJson.vehicleInvoiceBuyer == custName) {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayVehicleInvoiceObj.push("购买方姓名");
            }

            compareArr = startCompare(vehicleInvoiceJson.vehicleInvoiceBuyerId, certNo);
            vehicleInvoiceResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">证件号码</td>\
                                            <td class=\"col-md-2\">"+ compareArr[0].join('') + "</td>\
                                            <td class=\"col-md-2\">"+ compareArr[1].join('') + "</td>";
            if (vehicleInvoiceJson.vehicleInvoiceBuyerId == certNo) {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayVehicleInvoiceObj.push("证件号码");
            }
            vehicleInvoiceResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">发票金额</td>\
                                            <td class=\"col-md-2\">"+ vehicleInvoiceJson.vehicleInvoiceTotalPrice + "</td>\
                                            <td class=\"col-md-2\">"+ asset_sale_price + "</td>";
            if (vehicleInvoiceJson.vehicleInvoiceTotalPrice == asset_sale_price) {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayVehicleInvoiceObj.push("发票金额");
            }

            compareArr = startCompare(vehicleInvoiceJson.vehicleInvoiceCarVin, vinNumber);
            vehicleInvoiceResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">车架号</td>\
                                            <td class=\"col-md-2\">"+ compareArr[0].join('') + "</td>\
                                            <td class=\"col-md-2\">"+ compareArr[1].join('') + "</td>";
            if (vehicleInvoiceJson.vehicleInvoiceCarVin == vinNumber) {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayVehicleInvoiceObj.push("车架号");
            }
            vehicleInvoiceResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">销货单位</td>\
                                            <td class=\"col-md-2\">"+ vehicleInvoiceJson.vehicleInvoiceDealer + "</td>\
                                            <td class=\"col-md-2\">"+ jxs + "</td>";
            if (vehicleInvoiceJson.vehicleInvoiceDealer == jxs) {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                vehicleInvoiceResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayVehicleInvoiceObj.push("销货单位");
            }
            vehicleInvoiceResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">开票日期</td>\
                                            <td class=\"col-md-2\">"+ vehicleInvoiceJson.vehicleInvoiceIssueDate + "</td>\
                                            <td class=\"col-md-2\">"+ receiveTime + "</td>\
                                            <td class=\"col-md-2\">-</td>";
            vehicleInvoiceResultInfoHtml += "</tr>";
        }
        $("#vehicleInvoiceResultInfo").append(vehicleInvoiceResultInfoHtml);

        length = arrayVehicleInvoiceObj.length;
        // 如果长度为0，说明识别结果一致，不需要处理
        if (length !== 0) {
            vehicleInvoiceResult = '<tr><td rowspan="' + Math.max(1, Math.ceil(length / 3)) + '" class="col-md-2">发票</td>';
            $.each(arrayVehicleInvoiceObj, function (index, value) {
                if ((index + 3) % 3 === 0 && index > 0) {
                    vehicleInvoiceResult += "</tr><tr>";
                }
                vehicleInvoiceResult += '<td class="col-md-2">' + value + '：<span style="color: red">不一致</span></td>';
            });

            // 补齐 <td> 标签
            if (length % 3 !== 0) {
                counter = (Math.ceil(length / 3) * 3 - length);
                for (var i = 0; i < counter; i++) {
                    vehicleInvoiceResult += '<td></td>';
                }
            }

            vehicleInvoiceResult += "</tr>";
            $("#ocr").append(vehicleInvoiceResult);
        }
    }

    // 绑定合同HTML
    function bindContractHtml(instanceId, applicationNumber, vinNumber) {
        var url = '/Portal/OCR/GetContractResult?instanceID=' + instanceId;
        var contractJson = getData(url);
        var vehiclePrice = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ASSET_COST", 1);
        //获取共借人信息
        var gongjieren_xingming = $("#gjr_name").find("span").html();
        var gongjieren_idCard = $("#gjr_id").find("span").html();
        var dkje = smalltoBIG($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1), "元");
        var sfkje = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CASH_DEPOSIT", 1);
        var term = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1);
        //var khll = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BASE_CUSTOMER_RATE", 1);
        var khll = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACTUAL_RTE", 1);
        var assureMeans = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.GURANTEE_OPTION_H3", 1);//担保方式
        var counter, length, contractResult, contractResultInfoHtml = "", arrayContractObj = [], compareArr;

        if ($.isEmptyObject(contractJson)) {
            contractResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">合同编号</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ applicationNumber + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">借款人姓名</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ custName + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">身份证号</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ certNo + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">共借人姓名</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ gongjieren_xingming + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">共借人身份证号</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ gongjieren_idCard + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">车价及附加费</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ vehiclePrice + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">贷款本金（大写）</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ dkje + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">借款人首付</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ sfkje + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">期限</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ term + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">固定利率</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ khll + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">担保方式</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ assureMeans + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">车架号</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ vinNumber + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>";
            arrayContractObj.push("合同编号");
            arrayContractObj.push("借款人姓名");
            arrayContractObj.push("借款人身份证号");
            arrayContractObj.push("共借人姓名");
            arrayContractObj.push("共借人身份证号");
            arrayContractObj.push("车价及附加费");
            arrayContractObj.push("贷款本金（大写）");
            arrayContractObj.push("借款人首付");
            arrayContractObj.push("期限");
            arrayContractObj.push("固定利率");
            arrayContractObj.push("担保方式");
            arrayContractObj.push("车架号");
        } else {
            contractResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">合同编号</td>\
                                            <td class=\"col-md-2\">"+ contractJson.hetongbianhao + "</td>\
                                            <td class=\"col-md-2\">"+ applicationNumber + "</td>";
            if (contractJson.hetongbianhao == applicationNumber) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("合同编号");
            }
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">借款人姓名</td>\
                                            <td class=\"col-md-2\">"+ contractJson.jiekuanren_xingming + "</td>\
                                            <td class=\"col-md-2\">"+ custName + "</td>";
            if (contractJson.jiekuanren_xingming == custName) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("借款人姓名");
            }

            compareArr = startCompare(contractJson.jiekuanren_zhengjianhaoma, certNo);
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">借款人身份证号</td>\
                                            <td class=\"col-md-2\">"+ compareArr[0].join('') + "</td>\
                                            <td class=\"col-md-2\">"+ compareArr[1].join('') + "</td>";

            if (contractJson.jiekuanren_zhengjianhaoma == certNo) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("借款人身份证号");
            }
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">共借人姓名</td>\
                                            <td class=\"col-md-2\">"+ contractJson.gongjieren_xingming + "</td>\
                                            <td class=\"col-md-2\">"+ gongjieren_xingming + "</td>";
            if (contractJson.gongjieren_xingming == gongjieren_xingming) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("共借人姓名");
            }

            compareArr = startCompare(contractJson.gongjieren_zhengjianhaoma, gongjieren_idCard)
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">共借人身份证号</td>\
                                            <td class=\"col-md-2\">"+ compareArr[0].join('') + "</td>\
                                            <td class=\"col-md-2\">"+ compareArr[1].join('') + "</td>";
            if (contractJson.gongjieren_zhengjianhaoma == gongjieren_idCard) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("共借人身份证号");
            }

            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">车价及附加费</td>\
                                            <td class=\"col-md-2\">"+ (parseFloat(contractJson.chejiajine.replace(",", "")) + parseFloat(contractJson.fujiafeijine.replace(",", "")) || "") + "</td>\
                                            <td class=\"col-md-2\">"+ vehiclePrice + "</td>";
            if ((parseFloat(contractJson.chejiajine.replace(",", "")) + parseFloat(contractJson.fujiafeijine.replace(",", ""))) == vehiclePrice) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("车价及附加费");
            }
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">贷款本金（大写）</td>\
                                            <td class=\"col-md-2\">"+ contractJson.daikuanbenjin_daxie + "</td>\
                                            <td class=\"col-md-2\">"+ dkje + "</td>";
            if (contractJson.daikuanbenjin_daxie == dkje) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("贷款本金（大写）");
            }
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">借款人首付</td>\
                                            <td class=\"col-md-2\">"+ contractJson.jiekuanrenshoufukuanjine.replace(",", "") + "</td>\
                                            <td class=\"col-md-2\">"+ sfkje + "</td>";
            if (contractJson.jiekuanrenshoufukuanjine.replace(",", "") == sfkje) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("借款人首付");
            }
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">期限</td>\
                                            <td class=\"col-md-2\">"+ contractJson.daikuanqixian + "</td>\
                                            <td class=\"col-md-2\">"+ term + "</td>";
            if (contractJson.daikuanqixian == term) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("期限");
            }
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">固定利率</td>\
                                            <td class=\"col-md-2\">"+ contractJson.daikuanlilv + "</td>\
                                            <td class=\"col-md-2\">"+ khll + "</td>";
            if (contractJson.daikuanlilv == khll) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("固定利率");
            }
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">担保方式</td>\
                                            <td class=\"col-md-2\">"+ contractJson.danbaofangshi + "</td>\
                                            <td class=\"col-md-2\">"+ assureMeans + "</td>";
            if (contractJson.danbaofangshi == assureMeans) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("担保方式");
            }

            compareArr = startCompare(contractJson.chejiahao, vinNumber);
            contractResultInfoHtml += "</tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">车架号</td>\
                                            <td class=\"col-md-2\">"+ compareArr[0].join('') + "</td>\
                                            <td class=\"col-md-2\">"+ compareArr[1].join('') + "</td>";
            if (contractJson.chejiahao == vinNumber) {
                contractResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                contractResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayContractObj.push("车架号");
            }
            contractResultInfoHtml += "</tr>";
        }

        $("#idContractResultInfo").append(contractResultInfoHtml);

        length = arrayContractObj.length;
        if (length !== 0) {
            contractResult = "<tr><td rowspan=\"" + Math.max(1, Math.ceil(length / 3)) + "\" class=\"col-md-2\">合同</td>";
            $.each(arrayContractObj, function (index, value) {
                if ((index + 3) % 3 == 0 && index > 0) {
                    contractResult += "</tr><tr>";
                }
                contractResult += "<td class=\"col-md-2\">" + value + "：<span style=\"color: red\">不一致</span></td>";
            });

            // 补齐 <td> 标签
            if (length % 3 !== 0) {
                counter = (Math.ceil(length / 3) * 3 - length);
                for (var i = 0; i < counter; i++) {
                    contractResult += '<td></td>';
                }
            }

            contractResult += "</tr>";
            $("#ocr").append(contractResult);
        }
    }

    // 绑定扣款授权书HTML
    function bindDeductionsAuthorizedHtml(instanceId, applicationNumber, accountNumber) {
        var url = '/Portal/OCR/GetDeductionsAuthorizedResult?instanceID=' + instanceId;
        var deductionsAuthorizedJson = getData(url);
        var branchName = $.MvcSheetUI.GetControlValue("Branchname");
        var counter, length, deductionsAuthorizedResult, deductionsAuthorizedResultInfoHtml = "", arrayDeductionsAuthorizedObj = [], compareArr;

        if ($.isEmptyObject(deductionsAuthorizedJson)) {
            deductionsAuthorizedResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">申请号</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ applicationNumber + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">客户名</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ custName + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">银行分支</td>\
                                            <td class=\"col-md-2\">-</td>\
                                            <td class=\"col-md-2\">"+ branchName + "</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">账户</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ accountNumber + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">身份证号</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ certNo + "</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">证件类型</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">-</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>";
            arrayDeductionsAuthorizedObj.push("申请号");
            arrayDeductionsAuthorizedObj.push("客户名");
            arrayDeductionsAuthorizedObj.push("账户");
            arrayDeductionsAuthorizedObj.push("身份证号");
            arrayDeductionsAuthorizedObj.push("证件类型");
        } else {
            deductionsAuthorizedResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">申请号</td>\
                                            <td class=\"col-md-2\">"+ deductionsAuthorizedJson.id + "</td>\
                                            <td class=\"col-md-2\">"+ applicationNumber + "</td>";
            if (deductionsAuthorizedJson.id == applicationNumber) {
                deductionsAuthorizedResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                deductionsAuthorizedResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayDeductionsAuthorizedObj.push("申请号");
            }
            deductionsAuthorizedResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">客户名</td>\
                                            <td class=\"col-md-2\">"+ deductionsAuthorizedJson.customer + "</td>\
                                            <td class=\"col-md-2\">"+ custName + "</td>";
            if (deductionsAuthorizedJson.customer == custName) {
                deductionsAuthorizedResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                deductionsAuthorizedResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayDeductionsAuthorizedObj.push("客户名");
            }

            compareArr = startCompare(deductionsAuthorizedJson.account, accountNumber);
            deductionsAuthorizedResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">银行分支</td>\
                                            <td class=\"col-md-2\">-</td>\
                                            <td class=\"col-md-2\">"+ branchName + "</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">账户</td>\
                                            <td class=\"col-md-2\">"+ compareArr[0].join('') + "</td>\
                                            <td class=\"col-md-2\">"+ compareArr[1].join('') + "</td>";
            if (deductionsAuthorizedJson.account == accountNumber) {
                deductionsAuthorizedResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                deductionsAuthorizedResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayDeductionsAuthorizedObj.push("账户");
            }

            compareArr = startCompare(deductionsAuthorizedJson.identity, certNo);
            deductionsAuthorizedResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">身份证号</td>\
                                            <td class=\"col-md-2\">"+ compareArr[0].join('') + "</td>\
                                            <td class=\"col-md-2\">"+ compareArr[1].join('') + "</td>";
            if (deductionsAuthorizedJson.identity == certNo) {
                deductionsAuthorizedResultInfoHtml += "<td class=\"col-md-2\">一致</td>";
            } else {
                deductionsAuthorizedResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>";
                arrayDeductionsAuthorizedObj.push("身份证号");
            }
            deductionsAuthorizedResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">证件类型</td>\
                                            <td class=\"col-md-2\">"+ deductionsAuthorizedJson.identityType + "</td>\
                                            <td class=\"col-md-2\">-</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>";
        }

        $("#idDeductionsAuthorizedInfo").append(deductionsAuthorizedResultInfoHtml);

        length = arrayDeductionsAuthorizedObj.length;
        if (length !== 0) {
            deductionsAuthorizedResult = "<tr><td rowspan=\"" + Math.max(1, Math.ceil(length / 3)) + "\" class=\"col-md-2\">扣款授权书</td>";
            $.each(arrayDeductionsAuthorizedObj, function (index, value) {
                if ((index + 3) % 3 == 0 && index > 0) {
                    deductionsAuthorizedResult += "</tr><tr>";
                }
                deductionsAuthorizedResult += "<td class=\"col-md-2\">" + value + "：<span style=\"color: red\">不一致</span></td>";
            });

            // 补齐 <td> 标签
            if (length % 3 !== 0) {
                counter = (Math.ceil(length / 3) * 3 - length);
                for (var i = 0; i < counter; i++) {
                    deductionsAuthorizedResult += '<td></td>';
                }
            }

            deductionsAuthorizedResult += "</tr>";
            $("#ocr").append(deductionsAuthorizedResult);
        }
    }

    // 绑定保单HTML
    function bindPolicyHtml(instanceId, vinNumber) {
        var url = '/Portal/OCR/GetPolicyResult?instanceID=' + instanceId;
        var policyJson = getData(url);
        var counter, length, policyResult, policyResultInfoHtml = "", arrayPolicyObj = [], compareArr;

        if ($.isEmptyObject(policyJson)) {
            policyResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">被保险人</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ custName + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">证件号码</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ certNo + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">车架号</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">"+ vinNumber + "</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">保险费合计</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">-</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">是否不计免赔</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">-</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">第三者责任险</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">-</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>\
                                        <tr>\
                                            <td class=\"col-md-2\">第一受益人</td>\
                                            <td class=\"col-md-2\"><span style=\"color: red\">无记录</span></td>\
                                            <td class=\"col-md-2\">-</td>\
                                            <td class=\"col-md-2\">-</td>\
                                        </tr>";
            arrayPolicyObj.push("被保险人");
            arrayPolicyObj.push("证件号码");
            arrayPolicyObj.push("车架号");
            arrayPolicyObj.push("保险费合计");
            arrayPolicyObj.push("是否不计免赔");
            arrayPolicyObj.push("第三者责任险");
            arrayPolicyObj.push("第一受益人");
        } else {
            policyResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">被保险人</td>\
                                            <td class=\"col-md-2\">"+ policyJson.beibaoxianren + "</td>\
                                            <td class=\"col-md-2\">"+ custName + "</td>";
            if (policyJson.beibaoxianren == custName) {
                policyResultInfoHtml += "<td class=\"col-md-2\">一致</td>\
                                        </tr>";
            } else {
                policyResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>";
                arrayPolicyObj.push("被保险人");
            }

            compareArr = startCompare(policyJson.zhengjianhaoma, certNo);
            policyResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">证件号码</td>\
                                            <td class=\"col-md-2\">"+ compareArr[0].join('') + "</td>\
                                            <td class=\"col-md-2\">"+ compareArr[1].join('') + "</td>";
            if (policyJson.zhengjianhaoma == certNo) {
                policyResultInfoHtml += "<td class=\"col-md-2\">一致</td>\
                                        </tr>";
            } else {
                policyResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>";
                arrayPolicyObj.push("证件号码");
            }

            compareArr = startCompare(policyJson.chejiahao, vinNumber);
            policyResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">车架号</td>\
                                            <td class=\"col-md-2\">"+ compareArr[0].join('') + "</td>\
                                            <td class=\"col-md-2\">"+ compareArr[1].join('') + "</td>";
            if (policyJson.chejiahao == vinNumber) {
                policyResultInfoHtml += "<td class=\"col-md-2\">一致</td>\
                                        </tr>";
            } else {
                policyResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">不一致</span></td>\
                                        </tr>";
                arrayPolicyObj.push("车架号");
            }
            policyResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">保险费合计</td>\
                                            <td class=\"col-md-2\">"+ policyJson.baofeiheji + "</td>\
                                            <td class=\"col-md-2\">-</td>";
            if (policyJson.baofeiheji == "") {
                policyResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">无</span></td>\
                                        </tr>";
                arrayPolicyObj.push("保险费合计");
            } else {
                policyResultInfoHtml += "<td class=\"col-md-2\">有</td>\
                                        </tr>";
            }
            policyResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">是否不计免赔</td>\
                                            <td class=\"col-md-2\">"+ policyJson.shifouyoubujimianpei + "</td>\
                                            <td class=\"col-md-2\">-</td>";
            if (policyJson.shifouyoubujimianpei == "" || policyJson.shifouyoubujimianpei == "no") {
                policyResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">无</span></td>\
                                        </tr>";
                arrayPolicyObj.push("是否不计免赔");
            } else {
                policyResultInfoHtml += "<td class=\"col-md-2\">有</td>\
                                        </tr>";
            }
            policyResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">第三者责任险</td>\
                                            <td class=\"col-md-2\">"+ policyJson.shifouyousanzexian + "</td>\
                                            <td class=\"col-md-2\">-</td>";
            if (policyJson.shifouyousanzexian == "" || policyJson.shifouyousanzexian == "no") {
                policyResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">无</span></td>\
                                        </tr>";
                arrayPolicyObj.push("第三者责任险");
            } else {
                policyResultInfoHtml += "<td class=\"col-md-2\">有</td>\
                                        </tr>";
            }
            policyResultInfoHtml += "<tr>\
                                            <td class=\"col-md-2\">第一受益人</td>\
                                            <td class=\"col-md-2\">"+ policyJson.diyishouyiren + "</td>\
                                            <td class=\"col-md-2\">-</td>";
            if (policyJson.diyishouyiren == "") {
                policyResultInfoHtml += "<td class=\"col-md-2\"><span style=\"color: red\">无</span></td>\
                                        </tr>";
                arrayPolicyObj.push("第一受益人");
            } else {
                policyResultInfoHtml += "<td class=\"col-md-2\">有</td>\
                                        </tr>";
            }
        }

        $("#idPolicyInfo").append(policyResultInfoHtml);

        length = arrayPolicyObj.length;
        if (length !== 0) {
            policyResult = "<tr><td rowspan=\"" + Math.max(1, Math.ceil(length / 3)) + "\" class=\"col-md-2\">保单</td>";
            $.each(arrayPolicyObj, function (index, value) {
                if ((index + 3) % 3 == 0 && index > 0) {
                    policyResult += "</tr><tr>";
                }
                if (value == "被保险人" || value == "证件号码" || value == "车架号") {
                    policyResult += "<td class=\"col-md-2\">" + value + "：<span style=\"color: red\">不一致</span></td>";
                } else {
                    policyResult += "<td class=\"col-md-2\">" + value + "：<span style=\"color: red\">无</span></td>";
                }
            });

            // 补齐 <td> 标签
            if (length % 3 !== 0) {
                counter = (Math.ceil(length / 3) * 3 - length);
                for (var i = 0; i < counter; i++) {
                    policyResult += '<td></td>';
                }
            }

            policyResult += "</tr>";
            $("#ocr").append(policyResult);
        }
    }

    // 获取基础信息
    function getData(url, method, dataType, contentType) {
        method = method || 'POST';
        dataType = dataType || 'json';
        contentType = contentType || 'application/json; charset=utf-8';
        var json;
        $.ajax({
            type: method,
            url: url,
            contentType: dataType,
            async: false,
            dataType: dataType,
            success: function (data) {
                if (data.Success) {
                    json = data.Data;
                } else {
                    shakeMsg(msg.responseText + "出错了");
                }
            },
            error: function (msg) {
                shakeMsg(msg.responseText + "出错了");
            }
        });
        return json;
    }
})(window);