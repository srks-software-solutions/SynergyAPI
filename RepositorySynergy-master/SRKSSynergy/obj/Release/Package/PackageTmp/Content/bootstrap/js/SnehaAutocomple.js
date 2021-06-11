
    $(document).ready(function () {
        $("#custname").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/LeadEnquiry/Autocomplete",
                    type: "POST",
                    dataType: "json",
                    data: { term: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item.MillName, value: item.MillName };
                        }))
                    }
                })
            },
            messages: {
                noResults: "", results: ""
            }
        });
    });

