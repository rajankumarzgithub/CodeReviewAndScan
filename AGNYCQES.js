var page_onLoad = function () {

    if (get$("USLCQES44").parent().parent())
        /*get$("USLCQES44").parent().append("<div class='QuestionRow'><div class = 'Question col-sm-4' style = 'margin-top:5px;'> <a target=\"_blank\" href=\"https://live.cloud.api.aig.com/life/connext-fdm/download/100AicF6FGkgO9MMYvefTIwGZMqWlYvYWj--oIBPb34xFxHEpiWuLT35q4EHmVzeR6Bj4fWtCNBe1N6Xda_QQVY93A\"><b>NY Reg 187 Bulletin</b></a> </div></div> ");*/
        get$("USLCQES44").parent().append("<div class='QuestionRow'><div class = 'Question col-sm-4' style = 'margin-top:5px;'> <a target=\"_blank\" href=\"https://live.cloud.api.corebridgefinancial.com/life/connext-fdm/download/100AicF6FGkgO9MMYvefTIwGZMqWlYvYWj--oIBPb34xFxHEpiWuLT35q4EHmVzeR6Bj4fWtCNBe1N6Xda_QQVY93A\"><b>NY Reg 187 Bulletin</b></a> </div></div> ");

    var OtherData;
    var GetWizardCrossPageOtherData = function () {
        var data = new Object();
        data.transactionID = $('#appID').val();
        data.stepCode = 'AGNYCQES';
        data.isQuestionnaire = true;
        var vAjaxTimeout = 60;
        var ajaxCall = new AjaxCall(ServiceURLS.Default, "GetWizardCrossPageOtherData", true, data);
        ajaxCall.invoke(GetWizardCrossPageOtherData_Success, GetWizardCrossPageOtherData_Faliure, vAjaxTimeout);
    };

    var GetWizardCrossPageOtherData_Success = function (result, context) {
        if (result.Success) {
            if (result.Data != null) {
                OtherData = $($.parseXML(result.Data));

                if (OtherData.find('IsChilRider').text() != "1") {
                    get$('AGCQES63').hide().clear_values();
                }
                if (OtherData.find('IsAdbRider').text() != "1") {
                    get$('AGCQES62').hide().clear_values();
                }

            }
        }
    };

    var GetWizardCrossPageOtherData_Faliure = function (result, context) {
    };
    GetWizardCrossPageOtherData();


    $(["USLCQES31_Label", "USLCQES32_Label", "USLCQES33_Label", "USLCQES34_Label", "USLCQES39_Label"]).each(function () {
        get$(this).remove();
    });

    $(["USLCQES26", "USLCQES29", "USLCQES30"]).each(function () {
        get$(this).removeClass("col-sm-12").addClass("col-sm-4");
    });

    $(["USLCQES27", "USLCQES28"]).each(function () {
        get$(this).removeClass("col-sm-12").addClass("col-sm-3");
    });

    $(["USLCQES29_Label", "USLCQES30_Label"]).each(function () {
        get$(this).find('label').html(get$(this).find('label').html() + "<span class=\"red-star\">*</span>");
    });

    $(["USLCQES27_Label", "USLCQES28_Label"]).each(function () {
        get$(this).find('label').css('margin-top', '7px');
    });

    //Removed add/delete buttons from rider section.
    get$('Riders').find('DIV.RepeatAction').remove();

    get$("ddl_AGCQES13").on("change", function () {
        AppContext.Wizard.DLState = get$('ddl_AGCQES13').val();
    });

    get$("rdo_AGCQES11_0").on("click", function () {
        $('#AGCQES11_Label').find('span:not(".red-star")').remove();
        $("#AGCQES11_Label").append('<span>' + "</br><b>Providing the driver's license number is an important factor in our underwriting review. Failure to provide upfront will result in case delays or unfavorable Express decisioning.</b>" + '</span>');
    });

    get$("rdo_AGCQES11_1").on("click", function () {
        $('#AGCQES11_Label').find('span:not(".red-star")').remove();
    });

    if ($("input[id='rdo_AGCQES11_0']:checked").val()) {
        $("#AGCQES11_Label").append('<span>' + "</br><b>Providing the driver's license number is an important factor in our underwriting review. Failure to provide upfront will result in case delays or unfavorable Express decisioning.</b>" + '</span>');
    };

    if (!get$('chk_USLCQES37_0').prop('checked'))
        get$('chk_USLCQES37_0').prop('checked', false).trigger('change');

    //get$("AGCQES121_Label").html($("<br/><span><b>Congratulations! Your case has qualified for Agile Underwriting+ (AU+). Post-issue reviews will be completed by our Underwriting team and any lack of material disclosure may result in policy rescission.Click to review <a target='_blank' href='https://adminplus.fgsfulfillment.com/View/AIGAG/1/AGLC110667-LB#page=7'>AU+ Criteria</a> </b></span>"));
    get$("AGCQES121_Label").html($("<br/><span><b>Congratulations! Your case has qualified for Agile Underwriting+ (AU+). Post-issue reviews will be completed by our Underwriting team and any lack of material disclosure may result in policy rescission.Click to review <a target='_blank' href='https://adminplus.fgsfulfillment.com/View/corebridgefinancial/1/AGLC110667-LB#page=7'>AU+ Criteria</a> </b></span>"));

};

var isSafeToClearValues = function (item) {
    var id = item.find('input').attr('id');
    switch (id) {
        case "rdo_AGCQES8_0":
            return false;
        case "rdo_AGCQES8_1":
            return false;
        case "txt_AGCQES9":
            return false;
        case "chk_USLCQES39_0":
        case "chk_USLCQES40_0":
        case "chk_USLCQES42_0":
        case "txt_USLCQES43":
        case "txt_USLCQES41":
            return false;
        default:
            return true;
    }
};