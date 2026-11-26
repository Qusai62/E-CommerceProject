import { Document, Page, StyleSheet, Text, View } from '@react-pdf/renderer';
import type { SupplierInvoiceDetailsResponse } from '@/shared/api/supplier-invoices-api';

const styles = StyleSheet.create({
  page: { padding: 42, fontSize: 10, color: '#172033', fontFamily: 'Helvetica' },
  top: { height: 6, backgroundColor: '#1d4ed8', marginBottom: 24 },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    borderBottomWidth: 2,
    borderBottomColor: '#183b82',
    paddingBottom: 18,
  },
  eyebrow: { fontSize: 9, color: '#1d4ed8', letterSpacing: 2, fontFamily: 'Helvetica-Bold' },
  title: { fontSize: 27, marginTop: 12, fontFamily: 'Helvetica-Bold' },
  muted: { color: '#5b677a', marginTop: 6 },
  right: { textAlign: 'right' },
  ref: { fontSize: 16, marginTop: 8, fontFamily: 'Helvetica-Bold' },
  facts: { flexDirection: 'row', justifyContent: 'space-between', paddingVertical: 22 },
  label: { fontSize: 8, color: '#64748b', fontFamily: 'Helvetica-Bold', letterSpacing: 0.7 },
  value: { marginTop: 6, fontSize: 12, fontFamily: 'Helvetica-Bold' },
  tableHead: {
    flexDirection: 'row',
    backgroundColor: '#eff6ff',
    color: '#183b82',
    paddingVertical: 9,
    fontSize: 8,
    fontFamily: 'Helvetica-Bold',
  },
  row: {
    flexDirection: 'row',
    paddingVertical: 11,
    borderBottomWidth: 1,
    borderBottomColor: '#e6edf8',
  },
  item: { width: '45%' },
  qty: { width: '12%', textAlign: 'right' },
  price: { width: '21.5%', textAlign: 'right' },
  total: { width: '21.5%', textAlign: 'right' },
  totals: {
    width: 210,
    marginLeft: 'auto',
    marginTop: 24,
    borderTopWidth: 2,
    borderTopColor: '#1d4ed8',
    paddingTop: 10,
  },
  totalRow: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 8 },
  outstanding: { fontSize: 13, fontFamily: 'Helvetica-Bold' },
  footer: {
    marginTop: 50,
    paddingTop: 18,
    borderTopWidth: 1,
    borderTopColor: '#d7e0ee',
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  signature: { width: 220 },
  signatureName: {
    marginTop: 18,
    minHeight: 25,
    borderBottomWidth: 1,
    borderBottomColor: '#64748b',
    paddingBottom: 5,
  },
});
const money = (value: number) => `JOD ${value.toFixed(3)}`;
const date = (value: string) =>
  new Intl.DateTimeFormat('en-JO', { dateStyle: 'medium' }).format(new Date(value));
export function SupplierInvoicePdf({
  invoice,
  signature,
}: {
  invoice: SupplierInvoiceDetailsResponse;
  signature: string;
}) {
  return (
    <Document title={`Supplier invoice ${invoice.invoiceNumber}`}>
      <Page size="A4" style={styles.page}>
        <View style={styles.top} />
        <View style={styles.header}>
          <View>
            <Text style={styles.eyebrow}>NEXORA · PROCUREMENT</Text>
            <Text style={styles.title}>SUPPLIER INVOICE</Text>
            <Text style={styles.muted}>
              Invoice #{invoice.supplierInvoiceId} · Created {date(invoice.createdAtUtc)}
            </Text>
          </View>
          <View style={styles.right}>
            <Text style={styles.label}>INVOICE REFERENCE</Text>
            <Text style={styles.ref}>{invoice.invoiceNumber}</Text>
            <Text style={styles.muted}>Invoice date: {date(invoice.invoiceDateUtc)}</Text>
          </View>
        </View>
        <View style={styles.facts}>
          <View>
            <Text style={styles.label}>SUPPLIER</Text>
            <Text style={styles.value}>
              {invoice.supplierName || `Supplier #${invoice.supplierId}`}
            </Text>
          </View>
          <View style={styles.right}>
            <Text style={styles.label}>PAYMENT STATUS</Text>
            <Text style={styles.value}>{invoice.paymentStatus}</Text>
          </View>
        </View>
        <View style={styles.tableHead}>
          <Text style={styles.item}>ITEM</Text>
          <Text style={styles.qty}>QTY</Text>
          <Text style={styles.price}>UNIT COST</Text>
          <Text style={styles.total}>LINE TOTAL</Text>
        </View>
        {invoice.items.map((item) => (
          <View key={item.supplierInvoiceItemId} style={styles.row}>
            <Text style={styles.item}>{item.productName}</Text>
            <Text style={styles.qty}>{item.quantity}</Text>
            <Text style={styles.price}>{money(item.unitPrice)}</Text>
            <Text style={styles.total}>{money(item.lineTotal)}</Text>
          </View>
        ))}
        <View style={styles.totals}>
          <View style={styles.totalRow}>
            <Text>Total</Text>
            <Text>{money(invoice.totalAmount)}</Text>
          </View>
          <View style={styles.totalRow}>
            <Text>Paid</Text>
            <Text>{money(invoice.paidAmount)}</Text>
          </View>
          <View style={styles.totalRow}>
            <Text style={styles.outstanding}>Outstanding</Text>
            <Text style={styles.outstanding}>{money(invoice.remainingAmount)}</Text>
          </View>
        </View>
        <View style={styles.footer}>
          <View style={styles.signature}>
            <Text style={styles.label}>AUTHORIZED SIGNATURE</Text>
            <Text style={styles.signatureName}>{signature || ' '}</Text>
            <Text style={styles.muted}>Signature / name</Text>
          </View>
          <Text style={[styles.muted, styles.right]}>
            This document is generated from the supplier invoice record.
          </Text>
        </View>
      </Page>
    </Document>
  );
}
