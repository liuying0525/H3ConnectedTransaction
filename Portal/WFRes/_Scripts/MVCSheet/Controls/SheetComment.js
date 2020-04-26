// SheetComment控件
(function ($) {
    //控件执行
    $.fn.SheetComment = function () {
        return $.MvcSheetUI.Run.call(this, "SheetComment", arguments);
    };

    $.MvcSheetUI.Controls.SheetComment = function (element, options, sheetInfo) {
        // 获取签章的显示位置，是放置在文本之前还是文本之后
        this.SignPosition = "BeforeComment"; // BeforeComment,AfterComment
        this.SignAlign = "Center";           // Left,Center,Right
        this.SignHeight = 0;                 // 签章高度,0表示自然高度
        this.SignWidth = 0;                  // 签章宽度,0表示自然宽度
        this.CommentFolded = false;          // 审批意见是否折叠，只针对移动端
        $.MvcSheetUI.Controls.SheetComment.Base.constructor.call(this, element, options, sheetInfo);
    };

    $.MvcSheetUI.Controls.SheetComment.Inherit($.MvcSheetUI.IControl, {
        Render: function () {
            //不可见返回 发起模式也不可见
            if (!this.Visiable) { //  || this.Originate
                this.Element.style.display = "none";
                return;
            }
            $(this.Element).addClass("SheetComment");

            //历史评论
            this.InitHistoryComment();

            // 不可编辑
            if (!this.Editable) {
                if (!this.NullCommentTitleVisible && (!this.V || !this.V.Comments || this.V.Comments.length === 0)) {
                    $("span[" + $.MvcSheetUI.PreDataKey + $.MvcSheetUI.DataFieldKey.toLowerCase() + "='" + this.DataField + "']").parent().hide();
                }
                return;
            }
            //评论输入
            this.InitCommentInput();
            // 常用意见和签章
            this.InitFrequentlyUsedCommentsAndSignature();
        },
        //移动端
        RenderMobile: function () {
            if (!this.Editable) {
                $(this.Element).closest(".item.item-input").hide();
                return;
            }//无编辑权限则隐
            var that = this;
            this.Render();
            this.Validate();

            $(this.Element).find(".widget-comments").hide();

            
            
            var bannerTitle = $('<div class="nav-icon fa fa-chevron-right bannerTitle"></div>');
            var bannerTitleLabel = $('<label id="commentTileLabel"></label>')
            bannerTitleLabel.text(SheetLanguages.Current.Comments);
            bannerTitleLabel.appendTo(bannerTitle);
            bannerTitle.insertBefore(this.Element);


            //将审批意见移动到最底部
            $(this.Element).closest('.list').appendTo($('#mobile-content div.scroll'));

            var oldDivContainer = $(this.Element).closest("div.item");
            oldDivContainer.css("padding", "0").removeClass("item-input");
            var spantitle = $(this.Element).parent().prev().remove();

            this.newDivTitle = $("<div class='item item-input item-icon-right'></div>");
            this.newDivTitle.append(spantitle);
            this.newDivTitle.insertBefore(oldDivContainer);
            if ($(this.Element).is(':hidden')) { this.newDivTitle.hide(); }

            ////展开审批意见
            //if (this.V && this.V.Comments) {
            //    if (this.V.Comments.length >= 1
            //        && this.V.Comments[0].Approval != -1) {
            //        this.expandTitle = "  折叠审批意见";
            //        this.expandComment = $("<i class='icon' style='font-size:100%;'><a class='aExpand' style='min-width:100px;' href='javascript:void(0)'><i class='ion-ios-arrow-down'></i>  " + this.expandTitle + "</a> </i>")
            //        this.newDivTitle.append(this.expandComment);
            //        $(this.Element).parent().width("100%");
            //        this.expandComment.unbind("click.Expand").bind("click.Expand", function () {
            //            var commentDiv = $(this).closest("div.item-input").next().find("div.widget-comments");
            //            if (commentDiv) {
            //                if (that.CommentFolded) {
            //                    var html = "<a class='aExpand' style='min-width:100px;' href='javascript:void(0)'><i class='ion-ios-arrow-up'></i>  收起审批意见</a>";
            //                    $(this).html(html)
            //                    that.CommentFolded = false;
            //                    commentDiv.show(1000);
            //                    if (!$(that.Element).hasClass("topBannerTitle")) {
            //                        setTimeout(function () {
            //                            $.MvcSheetUI.IonicFramework.$ionicScrollDelegate.scrollBy(0, 150, true);
            //                        }, 1000)
            //                    }
            //                } else {
            //                    var html = "<a class='aExpand' style='min-width:100px;' href='javascript:void(0)'><i class='ion-ios-arrow-down'></i>  展开审批意见</a>";
            //                    $(this).html(html)
            //                    that.CommentFolded = true;
            //                    commentDiv.hide(1000);
            //                    if (!$(that.Element).hasClass("topBannerTitle")) {
            //                        setTimeout(function () {
            //                            $.MvcSheetUI.IonicFramework.$ionicScrollDelegate.scrollBy(0, -150, true);
            //                        }, 1000)
            //                    }
            //                }
            //                $.MvcSheetUI.IonicFramework.$ionicScrollDelegate.resize();
            //            }
            //        })
            //    }
            //}
        },

        // 数据验证
        Validate: function (effective, initValid) {
            if (!this.Editable) return true;
            if (initValid) {
                if (this.Required && !this.GetValue()) {
                    this.AddInvalidText(this.Element, "*", false);
                    return false;
                }
            }
            if (!effective) {
                if (this.Required) {//必填的
                    if (!this.GetValue()) {
                        this.AddInvalidText(this.Element, "*");
                        return false;
                    }
                    else {
                        this.RemoveInvalidText(this.Element);
                    }
                }
            }
            return true;
        },

        GetValue: function () {
            return $(this.CommentInput).val();
        },
        _changeSignaturePic: function (panel, picName) {
            if (picName) {
                panel.html("<img border='0' src='" + $.MvcSheetUI.PortalRoot + "/TempImages/" + picName + ".jpg'>");
                var img = panel.find("img");
                if (this.SignWidth) {
                    img.width(this.SignWidth);
                }
                if (this.SignHeight) {
                    img.height(this.SignHeight);
                }
                // 签章过宽时，调整签章的宽度 用setTimeout是为了获取到宽度
                setTimeout(function () {
                    if (img.width() > panel.width()) {
                        img.width(panel.width());
                    }
                }, 100);
            }
            else {
                panel.html("");
            }
        },
        SetReadonly: function (flag) {
            if (flag) {
                $(this.Element).find("textarea").hide();
                $(this.Element).find("select").hide();
                $(this.Element).find(":checkbox").hide();
                $(this.Element).find("label").hide();
            }
            else {
                $(this.Element).find("textarea").show();
                $(this.Element).find("select").show();
                $(this.Element).find(":checkbox").show();
                $(this.Element).find("label").show();
            }
        },
        //返回数据值
        SaveDataField: function () {
            var result = {};
            if (!this.Visiable || !this.Editable) return result;
            result[this.DataField] = this.SheetInfo.BizObject.DataItems[this.DataField];
            if (!result[this.DataField]) {
                alert(SheetLanguages.Current.DataItemNotExists + ":" + this.DataField);
                return {};
            }

            var IsNewComment = false;

            // 签章
            var signatureId = "";
            if (this.SignatureSel) {
                signatureId = this.SignatureSel.val();
            }
            if (this.MyComment == undefined) {
                if (!this.GetValue()) return {};
                this.MyComment = {
                    CommentID: $.MvcSheetUI.NewGuid(),
                    UserName: SheetLanguages.Current.MyComment,
                    DateStr: new Date().toString(),
                    Text: this.GetValue(),
                    Avatar: $.MvcSheetUI.SheetInfo.UserImage,//$.MvcSheetUI.PortalRoot + "/assets/images/pixel-admin/user.jpg",
                    SignatureId: signatureId
                };
                IsNewComment = true;
                this.AddCommentItem(this.MyComment);
            }
                //else if (this.MyComment.Text == this.GetValue() && (this.MyComment.SignatureId || "") == signatureId) { //添加校验，如果值没变，就不会需要提交
                //    return {};
                //}
            else {
                var comment = $("#" + this.MyComment.CommentID);
                comment.find("div.comment-text").html(filterXSS(this.GetValue()));
                //修改签章
                var signature = comment.find("div.comment-signature");
                this._changeSignaturePic(signature, signatureId);
            }

            result[this.DataField].V = {
                CommentID: this.MyComment.CommentID,
                Text: this.GetValue(),
                IsNewComment: IsNewComment,
                SetFrequent: this.IsMobile ? false : $(this.SaveFrequentCk).is(":checked"),
                SignatureId: signatureId
            };

            return result;
        },

        //历史评论
        InitHistoryComment: function () {
            var that = this;
            if (this.V && this.V.Comments) {
                var commentLine = false;
                for (var i = 0; i < this.V.Comments.length; i++) {
                    if (this.LastestCommentOnly && i < this.V.Comments.length - 1) continue;
                    var commentObject = this.V.Comments[i];
                    if (commentObject.IsMyComment) {
                        this.MyComment = commentObject;
                        if (commentObject.DelegantName == "" && !this.IsMobile) {
                            this.MyComment.UserName = SheetLanguages.Current.MyComment;
                        }
                    }
                    commentLine = (this.V.Comments.length > 0 && i < this.V.Comments.length - 1);
                    if (commentObject.Approval != undefined && commentObject.Approval != -1) {
                        if (this.IsMobile) {
                            this.AddMobileCommentItem(commentObject, i == this.V.Comments.length - 1 ? true : false);
                        } else
                            this.AddCommentItem(commentObject, commentLine);
                    }
                }

                //添加收起审批意见DIV
                //if (this.IsMobile) {
                //    var foldCommentDiv = $("<div class='bottom-afold'><i class='icon'><a class='afold' href='javascript:void(0)' ng-click='HideHistoryComments()'><i class='ion-ios-arrow-up'></i>收起审批意见</a> </i> </div>")
                //    foldCommentDiv.find("a.afold").click("click.foldComment", function () {
                //        var html = "<a class='aExpand' style='min-width:100px;' href='javascript:void(0)'><i class='ion-ios-arrow-down'></i>  展开审批意见</a>";
                //        $(that.expandComment).html(html);
                //        that.CommentFolded = true;
                //        $(this).closest("div.widget-comments").hide(1000);
                //        $.MvcSheetUI.IonicFramework.$ionicScrollDelegate.resize();
                //        setTimeout(function () {
                //            $.MvcSheetUI.IonicFramework.$ionicScrollDelegate.scrollBy(0, -150, true);
                //        }, 1000)
                //    });
                //    $(this.Element).find("div.widget-comments").append(foldCommentDiv);
                //}
            }
        },
        //添加评论
        AddCommentItem: function (commentObject, commentLine) {
            if (this.PanelBody == undefined) {
                var CommentsPanel = $("<div class=\"widget-comments\"></div>");
                if (this.IsMobile) {
                    CommentsPanel.hide();
                }
                if (this.DisplayBorder) {
                    CommentsPanel.addClass("bordered");
                }
                this.PanelBody = $("<div></div>");
                CommentsPanel.append(this.PanelBody);//历史审批容器
                $(this.Element).prepend(CommentsPanel);
            }

            var commentItem = $("<div class='comment'></div>").attr("id", commentObject.CommentID);
            var commentBody = $("<div class='comment-body'></div>");
            if (!this.DisplayHead) commentBody.css("margin-left:3px");
            if (commentLine) commentBody.addClass("comment-line");

            // 是否显示头像
            if (this.DisplayHead) {
                var avatar = $("<img src='" + commentObject.Avatar + "' class='comment-avatar'></img>");
                commentItem.append(avatar);
            }
            else {
                commentBody.css({ "padding-left": 0 }); // pc端
                commentBody.css({ "margin-left": 0 }); // 移动端
            }
            var userName = commentObject.UserName;
            if (this.OUNameVisible && commentObject.OUName != undefined) {
                userName = commentObject.OUName + " " + userName;
            }

            //审批时间
            var dateStr = "";
            var modifyData = new Date(commentObject.DateStr);
            var today = new Date();
            //if (modifyData.getYear() == today.getYear()
            //    && modifyData.getMonth() == today.getMonth()
            //    && modifyData.getDate() == today.getDate()) {
            //    var hour = modifyData.getHours().toString();
            //    if (hour.length < 2) hour = "0" + hour;
            //    var minute = modifyData.getMinutes().toString();
            //    if (minute.length < 2) minute = "0" + minute;
            //    dateStr = SheetLanguages.Current.Today + hour + ":" + minute;
            //}
            //else {
                dateStr = commentObject.DateStr;
            //}

            if (commentObject.DelegantName) {
                userName = commentObject.DelegantName + "(" + userName + SheetLanguages.Current.Delegant + ")";
            }
            var commenby = $("<div class='comment-by'><a href='javascript:void(0)'>" + userName + "</a> </div>")
            if (commentObject.Activity && this.ActivityNameVisible) {
                var ActivityText = filterXSS(commentObject.Activity);
               
                commenby.append("[<a href='javascript:void(0)'>" + ActivityText + "</a>]");
            }
            var commenttext = $("<div class='comment-text'>" + filterXSS(commentObject.Text).replace(/\n/g, "<br>") + "</div>");
            if (this.IsMobile || this.DateNewLine) {
                commenby.append("<div style=\"padding-left:5px;\">" + dateStr + "</div>");
            }
            else {
                commenby.append("<span style=\"padding-left:5px;\">" + dateStr + "</span>");
            }
            if (this.SignPosition && this.SignPosition == "BeforeComment") {
                commentBody.append(commenby);
                commentBody.append(commenttext);
            }
            else {
                commentBody.append(commenttext);
                commenby.css("text-align", "right"); // 签名靠右对齐
                commentBody.append(commenby);
            }
            commentItem.append(commentBody);

            //操作：通过 or 驳回
            var approval = "";
            if (commentObject.Approval == 1) {
                approval = "操作：通过";
            } else if (commentObject.Approval == 0) {
                approval = "操作：驳回";
            }
            var commentApproval = $("<br><div class='comment-approval' style='float:right;margin-right:30px;'>" + approval + "</div><br>")
            commenttext.append(commentApproval)

            //签章
            var commentSignature = $("<div class='comment-signature' style='text-align:" + this.SignAlign + ";'></div>");
            commentBody.append(commentSignature);
            if (this.DisplaySign && commentObject.SignatureId) {
                this._changeSignaturePic(commentSignature, commentObject.SignatureId);
            }

            this.PanelBody.append(commentItem);
        },

        //添加评论
        AddMobileCommentItem: function (commentObject, last) {
            var Icon = "";
            var approvaltext = "";
            if (commentObject.Approval == 1) {  //通过
                Icon = "icon-comment-approval";
                approvaltext = "已同意";

            } else if (commentObject.Approval == 0) { //驳回
                Icon = "icon-comment-reject";
                approvaltext = "驳回";
            }
            if (this.PanelBody == undefined) {
                var CommentsPanel = $("<div class=\"widget-comments\"></div>");
                if (this.DisplayBorder) {
                    CommentsPanel.addClass("bordered");
                }
                this.PanelBody = $("<div class='list border-only-bottom'></div>");
                CommentsPanel.append(this.PanelBody); //历史审批容器
                $(this.Element).prepend(CommentsPanel);
            }
            //审批意见div
            var commentItem = $("<div class='comment-tab lefttriangle'></div>").attr("id", commentObject.CommentID);
            //时间轴
            var TimeAxis = last ? "" : "comment-time-axis";
            var commentTimeAxis = $("<i class='" + TimeAxis + "'></i><i class='" + Icon + "'></i>");
            //审批节点
            var commentActivity = $('<div class="comment-activity"><span >' + commentObject.Activity + '</span></div>');
            commentItem.append(commentTimeAxis);
            var commentDetails = $("<div class='comment-details'></div>");
            commentDetails.append(commentActivity);
            commentItem.append(commentDetails);
            //主体摘要
            var commentTitle = $("<div class='comment-title'></div>");
            var text = "";
            if (commentObject.Text) {
                //text = commentObject.Text.replace(/\n/g, "<br>");
                text = filterXSS(commentObject.Text).replace(/\n/g, "<br>");
            }
            //审批时间
            var dateStr = "";
            var modifyData = new Date(commentObject.DateStr);
            var today = new Date();
            //if (modifyData.getYear() == today.getYear()
            //    && modifyData.getMonth() == today.getMonth()
            //    && modifyData.getDate() == today.getDate()) {
            //    var hour = modifyData.getHours().toString();
            //    if (hour.length < 2) hour = "0" + hour;
            //    var minute = modifyData.getMinutes().toString();
            //    if (minute.length < 2) minute = "0" + minute;
            //    dateStr = SheetLanguages.Current.Today + hour + ":" + minute;
            //}
            //else {
                dateStr = commentObject.DateStr;
            //}
            var userName = commentObject.UserName;
            var Circlename = userName.substr(-2);
            if (this.OUNameVisible && commentObject.OUName != undefined) {
                userName = userName + " " + "<span class='comment-userOU'>" + commentObject.OUName + "</span>";
            }
            var commentCirclename = $("<i class='circle-name user-a'><span>" + Circlename + "</span></i>");
            var commentUsername = $("<span class='comment-user'>" + userName + "</span>");
            var commentDate = $("<span class='comment-date'>" + dateStr + "</span>");
            var commentApprovaltext = $("<div class='comment-approvaltext'>" + approvaltext + "</div>");
            commentTitle.append(commentCirclename).append(commentUsername).append(commentDate).append(commentApprovaltext);
            commentDetails.append(commentTitle);
            //审批内容
            var commentBody = $("<div class='comment-body'></div>");
            var commentText = $("<div class='comment-text'>" + filterXSS(text) + "</div>");
            commentBody.append(commentText);
            commentDetails.append(commentBody);
            this.PanelBody.append(commentItem);
        },
        //评论输入
        InitCommentInput: function () {
            var InputPanel = $("<div></div>");
            this.CommentInput = $("<textarea></textarea>").width(this.Width);

            if (this.IsMobile) {
                InputPanel.addClass('InputPanel item');
                var IPtitle = $("<div class='IPtitle'></div>");
                IPtitle.text(this.N);
                InputPanel.append(IPtitle);
                this.CommentInput = $("<textarea class='CommentInput' rows='4' placeholder='" + SheetLanguages.Current.InputYourComment + "'></textarea>");
                this.CommentInput.unbind('click').bind('click', function (e) {
                    e.stopPropagation();
                });
            }

            //已经有保存的评论
            if (this.MyComment) {
                //console.log(this.MyComment);
                this.CommentInput.val(filterXSS(this.MyComment.Text));
            }
            else {//默认评论
                //console.log(this.DefaultComment);
                this.CommentInput.val(filterXSS(this.DefaultComment));
            }
            InputPanel.append(this.CommentInput);
            $(this.Element).append(InputPanel);
            //值改变事件
            $(this.CommentInput).unbind("change.CommentInput").bind("change.CommentInput", this, function (e) {
                e.data.Validate.apply(e.data);
            });
        },

        //常用意见和签章
        InitFrequentlyUsedCommentsAndSignature: function () {
            var that = this;
            var ionic = $.MvcSheetUI.IonicFramework;
            var LatestCommentPanel = $("<div></div>").width(this.Width).css({ "text-align": "right" }),
                LatestCommentSel,
                SettingPanel,
                SignaturePanel,
                SignatureSel,
                SignaturePicPanel = $("<div></div>").width(this.Width).css({ "text-align": "right", "margin-top": "3px" });


            if (that.IsMobile) {
                LatestCommentPanel = $("<div></div>");
                LatestCommentPanel.addClass("item item-input LatestCommentPanel");
                SignaturePanel = $("<div></div>");
                SignaturePanel.addClass("item item-input SignaturePanel");
                $(this.Element).append(SignaturePanel);
                if (!this.Element.dataset['displaysign']) {
                    SignaturePanel.hide();
                }
            }

            $(this.Element).append(LatestCommentPanel);
            $(this.Element).append(SignaturePicPanel);

            // 常用意见下拉框
            if (this.FrequentCommentVisible) {
                if (!this.IsMobile) {
                    //pc端
                    LatestCommentSel = $("<select></select>");
                    LatestCommentSel.css({ width: "100%", "float": "left" });
                    LatestCommentSel.append("<option value=''>--" + SheetLanguages.Current.SelectComment + "--</option>");
                    if (this.V && this.V.FrequentlyUsedComments) {
                        for (var i = 0; i < this.V.FrequentlyUsedComments.length; i++) {
                            var text = this.V.FrequentlyUsedComments[i];
                            if (!text && text.length > 18) {
                                text = text.substring(0, 18) + "...";
                            }
                            var option = $("<option value='" + this.V.FrequentlyUsedComments[i] + "'>" + text + "</option>");
                            LatestCommentSel.append(option);
                        }
                    }
                    $(LatestCommentSel).unbind("change.LatestCommentSel").bind("change.LatestCommentSel", this, function (e) {
                        if ($(this).val().length > 0) {
                            e.data.CommentInput.val($(this).val());
                            e.data.Validate();
                        }
                    });
                    LatestCommentPanel.append(LatestCommentSel);
                } else {
                    //移动端
                    $("<span class='input-label'>" + SheetLanguages.Current.FreComments + "</span>").appendTo(LatestCommentPanel);
                    $("<div class='detail item-icon-right'><span class='input-label'>" + SheetLanguages.Current.PleaseSelectComment + "</span><i class='icon ion-ios-arrow-right'></i></div>").appendTo(LatestCommentPanel);
                    ionic.$scope.frequentCommentIndex=-1;
                    LatestCommentPanel.unbind('click.chooseComments').bind('click.chooseComments', function (e) {
                        e.stopPropagation();
                        ionic.$ionicModal.fromTemplateUrl('/Portal/Mobile/form/templates/radio_popover2.html', {
                            scope: ionic.$scope
                        }).then(function (popover) {

                            //console.log(popover.scope.$parent == ionic.$scope);
                            //console.log(popover.scope);
                            //console.log(ionic.$scope);
                            
                            popover.scope.header = SheetLanguages.Current.FreComments;
                            popover.scope.data = {};
                            var findex=ionic.$scope.frequentCommentIndex;
                            popover.scope.data.RadioListValue = findex;
                            popover.scope.RadioListDisplay = that.V.FrequentlyUsedComments;
                            popover.show();
                            popover.scope.hide = function () {
                                popover.hide();
                            }
                            if (this.IsMobile) {
                                $(popover.el).find(".popover").height("90%");
                            }
                            popover.scope.clickRadio = function (value, index) {
                                that.CommentInput.val(value);
                                that.Validate();
                                ionic.$scope.frequentCommentIndex = index;
                            };
                            popover.scope.searchFocus = false;
                            popover.scope.searchAnimate = function () {
                                popover.scope.searchFocus = !popover.scope.searchFocus;
                            };
                            popover.scope.searChange = function () {
                                popover.scope.searchNum = $(".active .popover .list").children('label').length;
                             
                            };
                        });
                        return;
                    });
                }
            }

            // 设为常用checkbox 移动端不显示
            if (this.FrequentSettingVisible && !this.IsMobile) {
                var checkboxid = $.MvcSheetUI.NewGuid();
                SettingPanel = $("<span></span>").css({ "margin-left": "10px" });
                this.SaveFrequentCk = $("<input type='checkbox'/>").attr("id", checkboxid);
                var Spantxt = $("<label type='checkbox' for='" + checkboxid + "'>" + SheetLanguages.Current.FrequentlyUsedComment + "</label>")
                Spantxt.css("cursor", "pointer");

                SettingPanel.append(this.SaveFrequentCk);
                SettingPanel.append(Spantxt);

                LatestCommentPanel.append(SettingPanel);

                if (LatestCommentSel) {
                    LatestCommentSel.width("80%");
                }
            }

            // 签章下拉框
            if (this.DisplaySign) {
                var mySignatures = $.MvcSheetUI.SheetInfo.MySignatures;
                var defaultSignatures;
                for (var i = 0; i < mySignatures.length;i++){
                    if (mySignatures[i].IsDefault) {
                        defaultSignatures = mySignatures[i];
                        break;
                    }
                }
                if (!this.IsMobile) {
                    //pc
                    SignatureSel = $("<select></select>").css({ "margin-left": "20px" });
                    SignatureSel.append("<option value=''>--" + SheetLanguages.Current.SelectSign + "--</option>");
                    if (mySignatures) {
                        for (var i = 0, len = mySignatures.length; i < len; i++) {
                            var signature = mySignatures[i];
                            var option = $("<option value='" + signature.ObjectID + "'>"
                                + (signature.Name.length > 18 ? signature.Name.substring(0, 18) + "..." : signature.Name)
                                + "</option>");
                            SignatureSel.append(option);
                            if (signature.IsDefault) SignatureSel.val(signature.ObjectID);
                        }
                        if (mySignatures.length == 1) SignatureSel.val(mySignatures[0].ObjectID);
                        that._changeSignaturePic(SignaturePicPanel, SignatureSel.val());
                    }

                    $(SignatureSel).unbind("change.signatureSel").bind("change.signatureSel", function () {
                        that._changeSignaturePic(SignaturePicPanel, $(this).val());
                    });

                    LatestCommentPanel.append(SignatureSel);


                    if (this.MyComment) {
                        SignatureSel.val(this.MyComment.SignatureId);
                        SignatureSel.trigger("change");
                    }

                    SignatureSel.width("20%");
                    if (LatestCommentSel) {
                        LatestCommentSel.width("55%");
                    }

                } else {
                    //移动端
                    $('<span class="input-label">' + SheetLanguages.Current.Signature + '</span>').appendTo(SignaturePanel);
                    $("<div class='detail item-icon-right'><span class='input-label'></span><i class='icon ion-ios-arrow-right'></i></div>").appendTo(SignaturePanel);

                    //val存储id
                    var SignatureSel = $('<input type="text" style="display:none;"></input>');
                    SignatureSel.appendTo(SignaturePanel);



                    //渲染默认的签章
                    if (defaultSignatures && defaultSignatures.SignatureID) {
                        SignatureSel.val(defaultSignatures.SignatureID);
                        SignatureSel.parents(".SignaturePanel").find('.detail .input-label').html("<img src=" + $.MvcSheetUI.PortalRoot + "/TempImages/" + defaultSignatures.SignatureID + ".jpg" + " />");
                    }

                    //如果有保存的评论
                    if (this.MyComment) {
                        SignatureSel.parents(".SignaturePanel").find('.detail .input-label').html("<img src=" + $.MvcSheetUI.PortalRoot + "/TempImages/" + this.MyComment.SignatureId + ".jpg" + " />");
                    }

                    SignaturePanel.unbind('click.chooseSignitrue').bind('click.chooseSignitrue', function (e) {
                        ionic.$ionicModal.fromTemplateUrl('Mobile/form/templates/radio_popover3.html', {
                            scope: ionic.$scope
                        }).then(function (popover) {
                            popover.scope.header = SheetLanguages.Current.Signature;
                            popover.scope.popover = popover;
                            popover.scope.RadioListDisplay = [];
                            popover.scope.SignatureImgs = [];
                            for (var i = 0; i < mySignatures.length; i++) {
                                popover.scope.RadioListDisplay[i] = {};
                                popover.scope.RadioListDisplay[i].id = mySignatures[i].SignatureID;
                                popover.scope.RadioListDisplay[i].name = mySignatures[i].Name;
                                popover.scope.SignatureImgs[i] = $.MvcSheetUI.PortalRoot + "/TempImages/" + mySignatures[i].SignatureID + ".jpg";
                            }
                            
                            
                            popover.scope.RadioListValue = SignatureSel.val();
                            
                            popover.show();
                            popover.scope.hide = function () {
                                popover.hide();
                            };
                            popover.scope.clickRadio = function (value) {
                                SignatureSel.val(value);
                                SignatureSel.parents(".SignaturePanel").find('.detail .input-label').html("<img src=" + $.MvcSheetUI.PortalRoot + "/TempImages/" + value + ".jpg" + " />");
                            };
                            popover.scope.searchFocus = false;
                            popover.scope.searchAnimate = function () {
                                popover.scope.searchFocus = !popover.scope.searchFocus;
                            };
                            popover.scope.searChange = function () {
                                popover.scope.searchNum = $(".active .popover .list").children('label').length;
                              
                            };
                        });
                        return;
                    });

                }
            }


            //保存表单时以SignatureSel.val()
            if (SignatureSel) {
                this.SignatureSel = SignatureSel;
            }
        }
    });
})(jQuery);