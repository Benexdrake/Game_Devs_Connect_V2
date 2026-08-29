import { PageContainer } from "@/components/ui";

export const metadata = { title: "Impressum – Gamedevs Connect" };

export default function ImpressumPage() {
  const name = process.env.IMPRESSUM_NAME;
  const address = process.env.IMPRESSUM_ADDRESS;
  const email = process.env.IMPRESSUM_EMAIL;

  if (!name || !address || !email) {
    return (
      <PageContainer width="md">
        <h1 className="mb-6 font-display text-sm text-accent-bright">IMPRESSUM</h1>
        <p className="text-sm text-danger">
          Impressum-Angaben fehlen. Bitte IMPRESSUM_NAME, IMPRESSUM_ADDRESS und IMPRESSUM_EMAIL in der .env setzen.
        </p>
      </PageContainer>
    );
  }

  const addressLines = address.split(",").map((line) => line.trim());

  return (
    <PageContainer width="md">
      <h1 className="mb-6 font-display text-sm text-accent-bright">IMPRESSUM</h1>

      <section className="mb-6">
        <h2 className="mb-2 font-display text-xs text-accent-bright">ANGABEN GEMÄSS § 5 TMG</h2>
        <p className="m-0 text-sm text-text">
          {name}
          <br />
          {addressLines.map((line, i) => (
            <span key={i}>
              {line}
              <br />
            </span>
          ))}
        </p>
      </section>

      <section className="mb-6">
        <h2 className="mb-2 font-display text-xs text-accent-bright">KONTAKT</h2>
        <p className="m-0 text-sm text-text">E-Mail: {email}</p>
      </section>

      <section className="mb-6">
        <h2 className="mb-2 font-display text-xs text-accent-bright">VERANTWORTLICH FÜR DEN INHALT NACH § 18 ABS. 2 MSTV</h2>
        <p className="m-0 text-sm text-text">
          {name}
          <br />
          {addressLines.map((line, i) => (
            <span key={i}>
              {line}
              <br />
            </span>
          ))}
        </p>
      </section>

      <section>
        <h2 className="mb-2 font-display text-xs text-accent-bright">EU-STREITSCHLICHTUNG</h2>
        <p className="m-0 text-sm text-text-muted">
          Die Europäische Kommission stellt eine Plattform zur Online-Streitbeilegung (OS) bereit:{" "}
          <a
            href="https://ec.europa.eu/consumers/odr/"
            target="_blank"
            rel="noopener noreferrer"
            className="text-accent hover:text-accent-bright"
          >
            https://ec.europa.eu/consumers/odr/
          </a>
          . Wir sind nicht verpflichtet und nicht bereit, an einem Streitbeilegungsverfahren vor einer
          Verbraucherschlichtungsstelle teilzunehmen.
        </p>
      </section>
    </PageContainer>
  );
}
