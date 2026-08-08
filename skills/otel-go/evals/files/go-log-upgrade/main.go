package main

import (
	"context"
	"time"

	"go.opentelemetry.io/otel/log"
)

func emit(ctx context.Context, logger log.Logger, userID string) {
	if logger.Enabled(ctx, log.EnabledParameters{Severity: log.SeverityInfo}) {
		var record log.Record
		record.SetTimestamp(time.Now())
		record.SetSeverity(log.SeverityInfo)
		record.SetBody(log.StringValue("signed in"))
		record.AddAttributes(log.String("user.id", userID))
		logger.Emit(ctx, record)
	}
}

func main() {}
