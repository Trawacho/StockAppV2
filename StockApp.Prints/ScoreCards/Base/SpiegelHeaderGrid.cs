using StockApp.Prints.Converters;
using System.Windows;
using System.Windows.Controls;

namespace StockApp.Prints.ScoreCards.Base
{
	internal class ScoreCardHeaderGrid : ScoreCardGrid
	{
		internal ScoreCardHeaderGrid(bool is8TurnsGame, bool opponentOnScoreCards)
			: base(is8TurnsGame, opponentOnScoreCards)
		{
			// Two Rows
			RowDefinitions.Add(new RowDefinition() { Height = new GridLength(PixelConverter.CmToPx(0.60)) });
			RowDefinitions.Add(new RowDefinition() { Height = new GridLength(PixelConverter.CmToPx(0.60)) });

			int colCounter = 0;
			int kehrenColSpan = opponentOnScoreCards
									? 6
									: is8TurnsGame
										? 8
										: 7;

			#region Texte Moarschaft (links)

			// fixe Felder links
			AddField(ref colCounter, "Bahn", 270, rowSpan: 2);
			AddField(ref colCounter, "Gegner", 270, rowSpan: 2);
			AddField(ref colCounter, "Anspiel", 270, rowSpan: 2);

			// "K e h r e n" Titel (ColumnSpan, aber colCounter NICHT vorverschieben)
			AddSpanTitle(colCounter, "K e h r e n", kehrenColSpan);

			// nummerierte Kehren (Row 1) — per Schleife statt Wiederholung
			AddKehrenGroup(ref colCounter, is8TurnsGame, include7and8: !opponentOnScoreCards);

			// Summe, ggf. Strafpunkte, Punkte
			var sumField = AddField(ref colCounter, "Summe", 0, rowSpan: 2);
			sumField.Textblock.FontWeight = FontWeights.Bold;

			if (!opponentOnScoreCards)
			{
				AddField(ref colCounter, "Straf-\r\npunkte", 0, rowSpan: 2);
			}

			var punkteField = AddField(ref colCounter, "Gewinn-\r\npunkte", 0, rowSpan: 2);
			punkteField.Textblock.FontWeight = FontWeights.Bold;

			#endregion

			// Abstand / Trennung wie im Original
			colCounter++;

			#region Texte Gegner (rechts)

			AddField(ref colCounter, "Spiel", 270, rowSpan: 2);

			AddSpanTitle(colCounter, "K e h r e n", kehrenColSpan);

			AddKehrenGroup(ref colCounter, is8TurnsGame, include7and8: !opponentOnScoreCards);

			var sumFieldG = AddField(ref colCounter, "Summe", 0, rowSpan: 2);
			sumFieldG.Textblock.FontWeight = FontWeights.Bold;

			if (!opponentOnScoreCards)
			{
				AddField(ref colCounter, "Straf-\r\npunkte", 0, rowSpan: 2);
			}

			var punkteFieldG = AddField(ref colCounter, "Gewinn-\r\npunkte", 0, rowSpan: 2);
			punkteFieldG.Textblock.FontWeight = FontWeights.Bold;

			if (opponentOnScoreCards)
			{
				colCounter++;
				AddField(ref colCounter, "Gegner", 0, rowSpan: 2);
			}

			#endregion
		}

		// Erstellt ein Feld, setzt Row/Column/RowSpan/ColumnSpan und erhöht col um colSpan.
		private ScoreCardField AddField(ref int col, string text, int? width = null, int row = 0, int rowSpan = 1, int colSpan = 1)
		{
			ScoreCardField field = width.HasValue ? new ScoreCardField(text, width.Value) : new ScoreCardField(text);
			if (rowSpan > 1) SetRowSpan(field, rowSpan);
			if (colSpan > 1) SetColumnSpan(field, colSpan);
			SetColumn(field, col);
			SetRow(field, row);
			Children.Add(field);
			col += colSpan;
			return field;
		}

		// Fügt einen Titel mit ColumnSpan an der aktuellen Startspalte ein, ohne colCounter zu ändern.
		private void AddSpanTitle(int startCol, string text, int span)
		{
			var title = new ScoreCardField(text);
			SetColumnSpan(title, span);
			SetColumn(title, startCol);
			SetRow(title, 0);
			Children.Add(title);
		}

		// Fügt die nummerierten Kehren (1..6 und optional 7/8) hinzu; erhöht col entsprechend.
		private void AddKehrenGroup(ref int col, bool is8TurnsGame, bool include7and8)
		{
			// Standard 1..6
			for (int i = 1; i <= 6; i++)
			{
				AddField(ref col, i.ToString(), row: 1);
			}

			if (include7and8)
			{
				AddField(ref col, "7", row: 1);
				if (is8TurnsGame)
				{
					AddField(ref col, "8", row: 1);
				}
			}
		}
	}
}
