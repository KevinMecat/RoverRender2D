using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoverRender2D.Application;
using RoverRender2D.Application.Abstractions;

namespace RoverRender2D.Desktop;

public sealed class MainForm : Form
{
    private static readonly Color Navy = Color.FromArgb(24, 38, 56);
    private static readonly Color SoftNavy = Color.FromArgb(34, 52, 72);
    private static readonly Color Accent = Color.FromArgb(46, 134, 99);
    private static readonly Color Canvas = Color.FromArgb(244, 247, 249);
    private static readonly Color MutedText = Color.FromArgb(91, 103, 116);

    private readonly IClock _clock;
    private readonly ILogger<MainForm> _logger;
    private readonly System.Windows.Forms.Timer _clockTimer;
    private readonly Label _clockLabel;

    public MainForm(
        IClock clock,
        IOptions<RoverRenderOptions> options,
        ILogger<MainForm> logger)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _clock = clock;
        _logger = logger;

        RoverRenderOptions settings = options.Value;
        Text = $"{settings.ApplicationName} — Base planimétrica 2D";
        AccessibleName = "Ventana principal de RoverRender2D";
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Canvas;
        Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        MinimumSize = new Size(960, 640);
        Size = new Size(1180, 760);
        StartPosition = FormStartPosition.CenterScreen;

        _clockLabel = CreateTextLabel(string.Empty, 9F, FontStyle.Regular, MutedText);
        _clockLabel.AutoSize = true;

        Controls.Add(CreateShell(settings));

        _clockTimer = new System.Windows.Forms.Timer { Interval = 1_000 };
        _clockTimer.Tick += HandleClockTick;
        UpdateClockLabel();
        _clockTimer.Start();

        Shown += HandleShown;
        FormClosed += HandleFormClosed;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _clockTimer.Dispose();
        }

        base.Dispose(disposing);
    }

    private Control CreateShell(RoverRenderOptions settings)
    {
        var shell = new TableLayoutPanel
        {
            BackColor = Canvas,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            RowCount = 2,
        };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 236F));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        Control header = CreateHeader(settings.ApplicationName);
        shell.Controls.Add(header, 0, 0);
        shell.SetColumnSpan(header, 2);
        shell.Controls.Add(CreateNavigation(), 0, 1);
        shell.Controls.Add(CreateHomeContent(settings), 1, 1);
        return shell;
    }

    private static Control CreateHeader(string applicationName)
    {
        var header = new TableLayoutPanel
        {
            BackColor = Navy,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(24, 0, 24, 0),
            RowCount = 1,
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        Label title = CreateTextLabel(applicationName, 17F, FontStyle.Bold, Color.White);
        title.Anchor = AnchorStyles.Left;
        title.AutoSize = true;

        var mode = new Label
        {
            Anchor = AnchorStyles.Right,
            AutoSize = true,
            BackColor = Accent,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold, GraphicsUnit.Point),
            Padding = new Padding(12, 7, 12, 7),
            Text = "MODO OFFLINE",
            TextAlign = ContentAlignment.MiddleCenter,
        };

        header.Controls.Add(title, 0, 0);
        header.Controls.Add(mode, 1, 0);
        return header;
    }

    private static Control CreateNavigation()
    {
        var navigation = new FlowLayoutPanel
        {
            AutoScroll = true,
            BackColor = SoftNavy,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Margin = Padding.Empty,
            Padding = new Padding(12, 22, 12, 12),
            WrapContents = false,
        };

        string[] items =
        [
            "Inicio",
            "Importar misión",
            "Calidad",
            "Procesamiento",
            "Mapa 2D",
            "Replay",
            "Exportación",
            "Configuración",
        ];

        for (int index = 0; index < items.Length; index++)
        {
            navigation.Controls.Add(CreateNavigationButton(items[index], index == 0));
        }

        return navigation;
    }

    private Control CreateHomeContent(RoverRenderOptions settings)
    {
        var content = new TableLayoutPanel
        {
            BackColor = Canvas,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(38, 34, 38, 24),
            RowCount = 4,
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        Label heading = CreateTextLabel("Bienvenido", 23F, FontStyle.Bold, Navy);
        heading.AutoSize = true;

        Label introduction = CreateTextLabel(
            "Importa una misión registrada por el rover para validarla y procesarla completamente sin conexión.",
            11F,
            FontStyle.Regular,
            MutedText);
        introduction.AutoSize = true;
        introduction.Margin = new Padding(0, 8, 0, 28);
        introduction.MaximumSize = new Size(760, 0);

        content.Controls.Add(heading, 0, 0);
        content.Controls.Add(introduction, 0, 1);
        content.Controls.Add(CreateStatusCard(settings), 0, 2);
        content.Controls.Add(CreateFooter(), 0, 3);
        return content;
    }

    private static Control CreateStatusCard(RoverRenderOptions settings)
    {
        var card = new TableLayoutPanel
        {
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
            BackColor = Color.White,
            ColumnCount = 1,
            Margin = Padding.Empty,
            MaximumSize = new Size(820, 250),
            MinimumSize = new Size(520, 220),
            Padding = new Padding(26),
            RowCount = 4,
        };
        card.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        card.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        card.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        card.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        card.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        Label ready = CreateTextLabel("Aplicación lista", 15F, FontStyle.Bold, Navy);
        ready.AutoSize = true;

        Label detail = CreateTextLabel(
            "No hay una misión cargada. La fuente original nunca se modifica durante la importación.",
            10F,
            FontStyle.Regular,
            MutedText);
        detail.AutoSize = true;
        detail.Margin = new Padding(0, 8, 0, 18);
        detail.MaximumSize = new Size(690, 0);

        var importButton = new Button
        {
            AutoSize = true,
            BackColor = Accent,
            Cursor = Cursors.Hand,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            Padding = new Padding(14, 8, 14, 8),
            Text = "Importar misión",
            UseVisualStyleBackColor = false,
        };
        importButton.FlatAppearance.BorderSize = 0;
        importButton.Click += (_, _) => MessageBox.Show(
            "El flujo seguro de importación se incorporará en el siguiente incremento.",
            settings.ApplicationName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        Label offlineNotice = CreateTextLabel(
            "Los datos se procesan localmente; esta aplicación no muestra telemetría en vivo.",
            9F,
            FontStyle.Italic,
            MutedText);
        offlineNotice.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        offlineNotice.AutoSize = true;

        card.Controls.Add(ready, 0, 0);
        card.Controls.Add(detail, 0, 1);
        card.Controls.Add(importButton, 0, 2);
        card.Controls.Add(offlineNotice, 0, 3);
        return card;
    }

    private Control CreateFooter()
    {
        var footer = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 18, 0, 0),
            RowCount = 1,
        };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        Label status = CreateTextLabel("Listo · Sin misión cargada", 9F, FontStyle.Regular, MutedText);
        status.AutoSize = true;
        status.Anchor = AnchorStyles.Left;
        _clockLabel.Anchor = AnchorStyles.Right;

        footer.Controls.Add(status, 0, 0);
        footer.Controls.Add(_clockLabel, 1, 0);
        return footer;
    }

    private static Button CreateNavigationButton(string text, bool selected)
    {
        var button = new Button
        {
            AccessibleName = $"Abrir {text}",
            BackColor = selected ? Navy : SoftNavy,
            FlatStyle = FlatStyle.Flat,
            ForeColor = selected ? Color.White : Color.FromArgb(211, 220, 229),
            Height = 44,
            Margin = new Padding(0, 0, 0, 4),
            Padding = new Padding(14, 0, 0, 0),
            Text = text,
            TextAlign = ContentAlignment.MiddleLeft,
            UseVisualStyleBackColor = false,
            Width = 208,
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private static Label CreateTextLabel(string text, float size, FontStyle style, Color color) =>
        new()
        {
            ForeColor = color,
            Font = new Font("Segoe UI", size, style, GraphicsUnit.Point),
            Text = text,
        };

    private void HandleClockTick(object? sender, EventArgs e) => UpdateClockLabel();

    private void HandleShown(object? sender, EventArgs e) =>
        _logger.LogInformation("RoverRender2D desktop shell started in offline mode at {UtcTime}.", _clock.UtcNow);

    private void HandleFormClosed(object? sender, FormClosedEventArgs e) =>
        _logger.LogInformation("RoverRender2D desktop shell closed.");

    private void UpdateClockLabel() =>
        _clockLabel.Text = $"UTC {_clock.UtcNow:yyyy-MM-dd HH:mm:ss}";
}
