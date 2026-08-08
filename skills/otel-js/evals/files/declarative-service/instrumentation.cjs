const { startNodeSDK } = require('@opentelemetry/sdk-node');
const { getNodeAutoInstrumentations } = require('@opentelemetry/auto-instrumentations-node');

startNodeSDK({
  instrumentations: [getNodeAutoInstrumentations()],
});
