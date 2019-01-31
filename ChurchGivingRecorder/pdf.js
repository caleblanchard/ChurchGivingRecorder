module.exports = function (callback, data) {
    var jsreport = require('jsreport-core')();

    jsreport.init().then(function () {
        return jsreport.render({
            template: {
                content: '{{:foo}}',
                engine: 'jsrender',
                recipe: 'chrome-pdf',
                chrome: {
                    marginTop: '100px',
                    marginBottom: '100px',
                    marginLeft: '75px',
                    marginRight: '75px'
                }
            },
            data: {
                foo: data
            }
        }).then(function (resp) {
            callback(/* error */ null, resp.content.toJSON().data);
        });
    }).catch(function (e) {
        callback(/* error */ e, null);
    })
};