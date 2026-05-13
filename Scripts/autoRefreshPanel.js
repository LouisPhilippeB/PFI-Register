/////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Author: Nicolas Chourot
// 2026
//
// Dependances :
//     - jquery version > 3.0
//     - bootbox
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////

let DefaultPeriodicRefreshRate = 5;
let EndSessionAction = '/Accounts/Login';
let timerHideUpdateView = null;

class AutoRefreshedPanel {
    constructor(panelId, contentServiceURL, refreshRate = DefaultPeriodicRefreshRate, postRefreshCallback = null) {
        this.contentServiceURL = contentServiceURL;
        this.panelId = panelId;
        this.postRefreshCallback = postRefreshCallback;
        this.previousScrollPosition = 0;
        this.paused = false;

        if (refreshRate != -1) {
            this.refresh(true);
            this.refreshRate = refreshRate * 1000;
            setInterval(() => {
                $("#updatingView").show();
                this.refresh();
            }, this.refreshRate);
        }

        $("#updatingView").hide();
    }

    pause() {
        this.paused = true;
    }

    restart() {
        this.paused = false;
    }

    storeScrollPosition() {
        this.previousScrollPosition = $("#mainContentPanel").scrollTop();
    }

    restoreScrollPosition() {
        $("#mainContentPanel").scrollTop(this.previousScrollPosition);
    }

    replaceContent(htmlContent) {
        if (htmlContent !== "") {
            this.storeScrollPosition();
            $("#" + this.panelId).html(htmlContent);
            this.restoreScrollPosition();

            if (this.postRefreshCallback != null) {
                this.postRefreshCallback();
            }
        }
    }

    redirect() {
        $("#updatingView").hide();

        if (EndSessionAction != "") {
            window.location = EndSessionAction + "?message=Votre session a été fermée.&success=false";
        }
        else {
            alert("Accès illégal!");
        }
    }

    refresh(forced = false) {
        if (!this.paused) {
            let url = this.contentServiceURL;

            if (forced) {
                url += this.contentServiceURL.indexOf("?") > -1 ? "&forceRefresh=true" : "?forceRefresh=true";
            }

            $.ajax({
                url: url,
                dataType: "html",
                success: (htmlContent) => {
                    if (htmlContent != "blocked") {
                        this.replaceContent(htmlContent);
                    }

                    clearTimeout(timerHideUpdateView);
                    timerHideUpdateView = setTimeout(() => { $("#updatingView").hide(); }, 500);
                },
                statusCode: {
                    401: this.redirect,
                    500: this.redirect
                }
            });
        }
    }

    command(url, moreCallBack = null) {
        $.ajax({
            url: url,
            method: 'GET',
            success: (params) => {
                this.refresh(true);

                if (moreCallBack != null) {
                    moreCallBack(params);
                }
            },
            statusCode: {
                401: this.redirect,
                500: this.redirect
            }
        });
    }

    postCommand(url, data, moreCallBack = null) {
        $.ajax({
            url: url,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: (params) => {
                this.refresh(true);

                if (moreCallBack != null) {
                    moreCallBack(params);
                }
            },
            statusCode: {
                401: this.redirect,
                500: this.redirect
            }
        });
    }

    confirmedCommand(message, url, moreCallBack = null) {
        bootbox.confirm(message, (result) => {
            if (result) {
                this.command(url, moreCallBack);
            }
        });
    }
}