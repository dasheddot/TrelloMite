using System;
using System.Collections.Generic;
using System.Linq;
using TrelloNet;
using Action = TrelloNet.Action;

namespace TrelloMite
{
    public class TrelloRunner : IDisposable
    {
        private readonly TrelloConfiguration _configuration;
        private readonly Trello _trello;

        public TrelloRunner(TrelloConfiguration configuration)
        {
            _trello = new Trello(configuration.AppKey);
            _configuration = configuration;

            _trello.Authorize(configuration.UserToken);
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        public void FindTimeEntries(Action<TimeEntry> saveTimeEntryCallback)
        {
            int cardsCount = 0;
            int commentsCount = 0;
            int successes = 0;
            int fails = 0;

            try
            {
                var boards = _trello.Boards.ForMe();
                var board =
                    boards.FirstOrDefault(
                        x => x.Name.Equals(_configuration.Board, StringComparison.InvariantCultureIgnoreCase));
                if (board == null)
                {
                    throw new ApplicationException("Board '" + _configuration.Board + "' not found.");
                }

                var allCards = _trello.Cards.ForBoard(board, BoardCardFilter.Open);

                foreach (var card in allCards)
                {
                    cardsCount++;

                    var allCommentActions = _trello.Actions.ForCard(card, new[] { ActionType.CommentCard });
                    var commentActionsCreatedByUser = allCommentActions.Cast<CommentCardAction>().Where(comment => comment.MemberCreator.Username.Equals(_configuration.UserName, StringComparison.InvariantCultureIgnoreCase));

                    foreach (var comment in commentActionsCreatedByUser)
                    {
                        string commentText = comment.Data.Text;
                        string newCommentText = "";

                        foreach (string oldLine in commentText.Split('\n').Select(x => x.Trim()))
                        {
                            string newLine = oldLine;

                            var parser = new TimeEntryParser();

                            TimeSpan duration;
                            if (parser.TryParseDuration(oldLine, out duration))
                            {
                                var timeEntry = new TimeEntry { Minutes = (int)duration.TotalMinutes };

                                string project;
                                if (parser.TryParseProject(card.Desc, out project)
                                    || parser.TryParseProject(commentText, out project))
                                {
                                    timeEntry.Project = project;
                                }

                                string customer;
                                if (parser.TryParseCustomer(card.Desc, out customer) || parser.TryParseCustomer(commentText, out customer))
                                {
                                    timeEntry.Customer = customer;
                                }

                                string service;
                                if (parser.TryParseService(card.Desc, out service) || parser.TryParseService(commentText, out service))
                                {
                                    timeEntry.Service = service;
                                }

                                string notes;
                                if (parser.TryParseNotes(card.Desc, out notes) || parser.TryParseNotes(commentText, out notes) || parser.TryParseNotes(card.Name, out notes))
                                {
                                    timeEntry.Notes = string.Format("{0} ({1})", notes ?? string.Empty, card.Url);
                                }
                                else
                                {
                                    timeEntry.Notes = card.Url;
                                }

                                Console.Write(
                                    "Found {0} minutes on {1} (customer: '{2}', project: '{3}', service: '{4}') ... ",
                                    timeEntry.Minutes,
                                    timeEntry.Date.ToShortDateString(),
                                    timeEntry.Customer,
                                    timeEntry.Project,
                                    timeEntry.Service);

                                try
                                {
                                    saveTimeEntryCallback.Invoke(timeEntry);
                                    newLine = newLine + " [mite ok]";
                                    _trello.Actions.ChangeText(comment, newCommentText.Trim());
                                    Console.WriteLine(" OK.");
                                    successes++;

                                    break;
                                }
                                catch (Exception ex)
                                {
                                    newLine = newLine + " [mite failed: " + ex.Message + "]";
                                    Console.WriteLine(" failed: " + ex.Message);
                                    fails++;
                                }
                            }

                            newCommentText += newLine + "\n";
                        }

                        commentsCount++;
                    }
                }
            }
            catch (TrelloUnauthorizedException exception)
            {
                var authorizationUrl = _trello.GetAuthorizationUrl("TrelloMite", Scope.ReadWrite, Expiration.Never);
                throw new ApplicationException(string.Format("Unable to connect to Trello. Make sure you authorized this app at {0}", authorizationUrl), exception);
            }

            Console.WriteLine("Scanned {0} cards and {1} comments, with {2} new entries and {3} failed.",
                              cardsCount,
                              commentsCount,
                              successes,
                              fails);
        }
    }
}