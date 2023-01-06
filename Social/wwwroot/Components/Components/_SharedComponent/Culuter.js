function getCookieValue(a) {
    var b = document.cookie.match('(^|[^;]+)\\s*' + a + '\\s*=\\s*([^;]+)');
    return b ? b.pop() : '';
}
var ResourcesFile = "";
var CurrentUserLanguage = GetCurrentUserLanguage();
LoadResources();
function getToken(key) {
    let Result = ResourcesFile[key];
    return Result == undefined ? key : Result;
}
function LoadResources(lang="")
{
    
    lang = lang == "" ? CurrentUserLanguage: lang;
    lang = (lang == "" || lang == undefined || lang == null) ? "ar-EG" : lang;
    return $.getJSON(`/Resources/${lang}.json`, function (data) {
        ResourcesFile = data;
    });
}
function GetCurrentUserLanguage() {
    let lang = getCookieValue('Usre_Culture');
    lang = (lang == "" || lang == undefined || lang == null) ? "ar-EG" : lang;
    return lang;
}
