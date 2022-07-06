<div class="col-md-2" style="justify-content:center;">
<div class="form-group form-group-sm label-floating input-group animated fadeInUp">
<label class="control-label" for="txtImporteAntesCopagoEdita">Importe A/Copago</label>
<input autocomplete="off" tabindex="43" type="text" id="txtImporteAntesCopagoEdition" name="txtImporteAntesCopagoEdita" class="form-control" required>
<span id="StatusAmountEdition" class="StatusAmount" aria-hidden="true"></span>
</div>
</div>




function ShowErrors(Errors) {
IsValidElegibilityEdition = true;
IsValidAuthorizationEdition = true;
IsValidFolioEdition = true;
IsValidSecondICD10Edition = true;
IsValidCopayEdition = true;
IsValidAmountBeforeCopay = true;
$('.StatusICD').removeClass().addClass('StatusICD glyphicon glyphicon-ok color-success');
$('.StatusConcept').removeClass().addClass('StatusConcept glyphicon glyphicon-ok color-success');
$('.StatusFolio').removeClass().addClass('StatusFolio glyphicon glyphicon-ok color-success');
$('.StatusAuthorization').removeClass().addClass('StatusAuthorization glyphicon glyphicon-ok color-success');
$('.StatusElegibility').removeClass().addClass('StatusElegibility glyphicon glyphicon-ok color-success');
$('.OriginProviderStatus').removeClass().addClass('OriginProviderStatus glyphicon glyphicon-ok color-success');
$('.StatusAmount').removeClass().addClass('StatusAmount glyphicon glyphicon-ok color-success');
$('.StatusDateOcurred').removeClass().addClass('StatusDateOcurred glyphicon glyphicon-ok color-success');
$('.StatusSecondICD10').removeClass().addClass('StatusSecondICD10 glyphicon glyphicon-ok color-success');
$('.StatusAmountCopay').removeClass().addClass('StatusAmountCopay glyphicon glyphicon-ok color-success');
$('.AnotherEnterprise').removeClass().addClass('AnotherEnterprise glyphicon glyphicon-ok color-success');
let _Elegibility = [1, 2, 3];
let _ICD10 = [4];
let _Concept = [15];
let _Folio = [8, 9, 18];
let _Authorization = [10, 11, 12, 27];
let _OriginProvider = [13, 14];
let _Amount = [16];
let _DateOcurred = [19, 22, 23, 24, 28, 30];
let _SecondICD10 = [5, 6];
let _Copay = [17];
let _AnotherEnterprise = [21, 25];
$.each(Errors, function (index, value) {
if (jQuery.inArray(value, _ICD10) !== -1) {
$('.StatusICD').removeClass().addClass('StatusICD glyphicon glyphicon-remove color-danger');
}



if (jQuery.inArray(value, _Concept) !== -1) {
InvalidConceptEdition = true;
$('.StatusConcept').removeClass().addClass('StatusConcept glyphicon glyphicon-remove color-danger');
$('.StatusAmount').removeClass().addClass('StatusAmount glyphicon glyphicon-remove color-danger');
}
else {
InvalidConceptEdition = false;
}



if (jQuery.inArray(value, _Folio) !== -1) {
$('.StatusFolio').removeClass().addClass('StatusFolio glyphicon glyphicon-remove color-danger');
if (value === 9) { //Folio marcado incorrecto
IsValidFolioEdition = false;
}
if (value === 8) { //Folio vacio
IsValidFolioEdition = false;
}
}
if (jQuery.inArray(value, _Authorization) !== -1) {
$('.StatusAuthorization').removeClass().addClass('StatusAuthorization glyphicon glyphicon-remove color-danger');
if (value === 12) {
IsValidAuthorizationEdition = false;
}
if (value === 27) {
IsValidAuthorizationEdition = false;
}
}
if (jQuery.inArray(value, _Elegibility) !== -1) {
$('.StatusElegibility').removeClass().addClass('StatusElegibility glyphicon glyphicon-remove color-danger');
}
if (jQuery.inArray(value, _OriginProvider) !== -1) {
$('.OriginProviderStatus').removeClass().addClass('OriginProviderStatus glyphicon glyphicon-remove color-danger');
}
if (jQuery.inArray(value, _Amount) !== -1) {
IsValidAmountBeforeCopay = false;
$('.StatusAmount').removeClass().addClass('StatusAmount glyphicon glyphicon-remove color-danger');
if (value === 16) {
IsValidAmountBeforeCopay = false;
}
}
if (jQuery.inArray(value, _DateOcurred) !== -1) {
$('.StatusDateOcurred').removeClass().addClass('StatusDateOcurred glyphicon glyphicon-remove color-danger');
}
if (jQuery.inArray(value, _SecondICD10) !== -1) {
$('.StatusSecondICD10').removeClass().addClass('StatusSecondICD10 glyphicon glyphicon-remove color-danger');
}
if (jQuery.inArray(value, _Copay) !== -1) {
IsValidCopayEdition = false;
$('.StatusAmountCopay').removeClass().addClass('StatusAmountCopay glyphicon glyphicon-remove color-danger');
}
if (jQuery.inArray(value, _AnotherEnterprise) !== -1) {
if (value === 21) {
$('.AnotherEnterprise').removeClass().addClass('AnotherEnterprise glyphicon glyphicon-remove color-danger');
$('.StatusElegibility').removeClass().addClass('StatusElegibility glyphicon glyphicon-remove color-danger');
}
if (value === 25) {
$('.AnotherEnterprise').removeClass().addClass('AnotherEnterprise glyphicon glyphicon-remove color-danger');
$('.StatusAuthorization').removeClass().addClass('StatusAuthorization glyphicon glyphicon-remove color-danger');
}
}
});



}