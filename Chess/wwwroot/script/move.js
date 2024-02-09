function submitMove(fromFile, fromRank, toFile, toRank, gameId) {
    var fromPosition = String.fromCharCode(97 + parseInt(fromFile)) + (parseInt(fromRank) + 1).toString();
    var toPosition = String.fromCharCode(97 + parseInt(toFile)) + (parseInt(toRank) + 1).toString();

    console.log(`Submitting move from (${fromPosition}) to (${toPosition})`);

    // Invoke the SubmitMove method on the server
    connectionSignalR.invoke("SubmitMove", gameId, fromPosition, toPosition)
        .then(function () {
            console.log("Move submitted successfully.");
        })
        .catch(function (err) {
            console.error("Error submitting move: ", err);
        });
}





document.addEventListener('DOMContentLoaded', function () {
    let selectedSquare = null;
    document.querySelectorAll('.square').forEach(square => {
        square.addEventListener('click', function () {
            // Highlight the selected square
            if (selectedSquare) {
                selectedSquare.classList.remove('selected');
            }
            if (selectedSquare === this) {
                // Deselect if the same square is clicked
                selectedSquare = null;
                return;
            }

            if (!selectedSquare) {
                // Selecting the first square (piece to move)
                this.classList.add('selected');
                selectedSquare = this;
            } else {
                // Selecting the target square (where to move)
                const fromFile = selectedSquare.getAttribute('data-file');
                const fromRank = selectedSquare.getAttribute('data-rank');
                const toFile = this.getAttribute('data-file');
                const toRank = this.getAttribute('data-rank');

                // Reset selection
                selectedSquare.classList.remove('selected');
                selectedSquare = null;

                submitMove(fromFile, fromRank, toFile, toRank, gameId);
            }
        });
    });
});