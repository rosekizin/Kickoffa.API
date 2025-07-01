import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import { QueryProvider } from "@/providers/query-client-provider";
import { ToastProvider } from "@/components/providers/toast-provider";
import { SessionManager } from "@/components/session/session-manager";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Kickoffa - Portal de Onboarding",
  description: "Portal de onboarding para freelancers com checklists e briefings estruturados",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="pt-BR">
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        <QueryProvider>
          <ToastProvider>
            <SessionManager
              enableKeepAlive={true}
              keepAlivePingInterval={900000}
              enableKeepAliveLogging={false}
            >
              {children}
            </SessionManager>
          </ToastProvider>
        </QueryProvider>
      </body>
    </html>
  );
}
